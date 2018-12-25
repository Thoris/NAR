using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using Reflector.Disassembler;

namespace Vil.Core
{
    public class Duplication
    {
        // Fields
        private int maxSize = 50;
        private int minSize = 0x19;
        private Hashtable patternMap = new Hashtable();
        private ModuleReader reader;

        // Methods
        public void ProcessAssembly(Assembly a)
        {
            foreach (Module module in a.GetModules())
            {
                this.reader = new ModuleReader(module, new AssemblyProvider());
                foreach (Type type in module.GetTypes())
                {
                    MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    for (int i = 0; i < methods.Length; i++)
                    {
                        MethodBase methodBase = methods[i];
                        Reflector.Disassembler.MethodBody methodBody = this.reader.GetMethodBody(methodBase);
                        if (methodBody != null)
                        {
                            this.ProcessMethod(methodBody);
                        }
                    }
                }
            }
        }

        private void ProcessMethod(Reflector.Disassembler.MethodBody methodBody)
        {
            Instruction[] instructions = methodBody.GetInstructions();
            int length = instructions.Length;
            for (int i = 0; i < (length - this.minSize); i++)
            {
                string key = "";
                if (this.minSize > 1)
                {
                    for (int k = i; k < ((i + this.minSize) - 1); k++)
                    {
                        key = key + instructions[k].Code.Name + '\t';
                    }
                }
                for (int j = this.minSize; j <= this.maxSize; j++)
                {
                    if ((i + j) >= length)
                    {
                        break;
                    }
                    key = key + instructions[i + j].Code.Name + '\t';
                    if (this.patternMap.ContainsKey(key))
                    {
                        this.patternMap[key] = ((int)this.patternMap[key]) + 1;
                    }
                    else
                    {
                        this.patternMap[key] = 1;
                    }
                }
            }
        }

        public void RenderAsText()
        {
            foreach (object obj2 in this.patternMap.Keys)
            {
                if (((int)this.patternMap[obj2]) > 3)
                {
                    Console.WriteLine(this.patternMap[obj2].ToString() + "\t" + ((string)obj2));
                }
            }
        }
    }


}