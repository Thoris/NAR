using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace NAR.Forms.Tests.Controls
{
    public partial class CommandList : UserControl
    {
        #region Events / Delegates

        public delegate void ListChanged(object sender, CommandListEventArgs e);
        public event ListChanged OnListChanged;

        #endregion

        #region Variables
        private ImageProcessing.CommandCollection _commands = new ImageProcessing.CommandCollection();
        private Model.IImage _comparedImage;
        #endregion

        #region Properties
        public ImageProcessing.CommandCollection Commands
        {
            get { return _commands; }
            set { _commands = value; }
        }
        public Model.IImage ComparedImage
        {
            get { return _comparedImage; }
            set { _comparedImage = value; }
        }
        #endregion

        #region Constructors/Destructors
        public CommandList()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        public void AddNewEntry(AvailableEntry entry)
        {
            this.chklList.Items.Add(entry, true);
        }
        private void RaiseEvent()
        {
            if (OnListChanged != null)
                OnListChanged(this, new CommandListEventArgs(GetList()));
        }
        public NAR.ImageProcessing.CommandCollection GetList()
        {
            ImageProcessing.CommandCollection collection = new ImageProcessing.CommandCollection();


            for (int c = 0; c < this.chklList.Items.Count; c++)
            {
                AvailableEntry entry = (AvailableEntry)this.chklList.Items[c];
                NAR.ImageProcessing.ICommand command = entry.CreateInstance();

                PropertyInfo [] properties = command.GetType().GetProperties();

                for (int p = 0; p < properties.Length; p++)
                {
                    if (string.Compare(properties[p].Name, "ImageBase", true) == 0)
                    {
                        properties[c].SetValue(command, _comparedImage, null);
                    }
                }

                collection.Add(command);
            }

            return collection;

        }
        private void ShowAttributes(AvailableEntry entry)
        {
            this.grdAttributes.Rows.Clear();

            ConstructorInfo[] constructors = entry.Command.GetConstructors();

            if (constructors == null || constructors.Length == 0)
                return;

            ParameterInfo [] parameters = constructors[0].GetParameters();

            if (parameters == null || parameters.Length == 0)
                return;

            for (int c=0;c < parameters.Length; c++)
            {
                this.grdAttributes.Rows.Add(
                    parameters[c].Name,
                    parameters[c].ParameterType.ToString(),
                    entry.Parameters[c].ToString());
            }
            
        }
        #endregion

        #region Events
        private void chklList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.grdAttributes.Rows.Clear();

            if (this.chklList.SelectedItem == null)
            {
                this.btnDelete.Enabled = false;
                this.btnDown.Enabled = false;
                this.btnUp.Enabled = false;

            }
            else
            {
                this.btnDelete.Enabled = true;
                this.btnDown.Enabled = true;
                this.btnUp.Enabled = true;

                if (this.chklList.SelectedIndex == 0)
                    this.btnUp.Enabled = false;
                else if (this.chklList.SelectedIndex == this.chklList.Items.Count - 1)
                    this.btnDown.Enabled = false;


                ShowAttributes((AvailableEntry) this.chklList.SelectedItem);


            }

        }
        private void btnUp_Click(object sender, EventArgs e)
        {
            int pos = this.chklList.SelectedIndex;
            AvailableEntry entry = (AvailableEntry) this.chklList.Items[pos];
            
            this.chklList.Items.RemoveAt(pos);
            this.chklList.Items.Insert(pos - 1, entry);
            
            this.chklList.SelectedIndex = pos - 1;


            RaiseEvent();

        }
        private void btnDown_Click(object sender, EventArgs e)
        {
            int pos = this.chklList.SelectedIndex;
            AvailableEntry entry = (AvailableEntry) this.chklList.Items[pos];

            this.chklList.Items.RemoveAt(pos);
            this.chklList.Items.Insert(pos + 1, entry);

            this.chklList.SelectedIndex = pos + 1;


            RaiseEvent();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            this.chklList.Items.RemoveAt(this.chklList.SelectedIndex);

            RaiseEvent();

        }
        private void chklList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Delete:

                    if (this.btnDelete.Enabled)
                        this.btnDelete_Click(this, EventArgs.Empty);

                    break;

            }
        }
        private void CommandList_Load(object sender, EventArgs e)
        {
        }
        private void grdAttributes_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {



        }
        private void grdAttributes_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != 2 || e.RowIndex < 0)
                return;

            string name = this.grdAttributes.Rows[e.RowIndex].Cells[0].Value.ToString();
            string type = this.grdAttributes.Rows[e.RowIndex].Cells[1].Value.ToString();
            string value = this.grdAttributes.Rows[e.RowIndex].Cells[2].Value.ToString();


            try
            {
                object result =  Convert.ChangeType(e.FormattedValue, Type.GetType(type));
            }
            catch
            {
                e.Cancel = true;
            }

        }
        private void grdAttributes_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 2 || e.RowIndex < 0)
                return;

            string name = this.grdAttributes.Rows[e.RowIndex].Cells[0].Value.ToString();
            string type = this.grdAttributes.Rows[e.RowIndex].Cells[1].Value.ToString();
            string value = this.grdAttributes.Rows[e.RowIndex].Cells[2].Value.ToString();


            try
            {
                object result = Convert.ChangeType(value, Type.GetType(type));

                ((AvailableEntry)this.chklList.SelectedItem).Parameters[e.RowIndex] = result ;

                RaiseEvent();

            }
            catch
            {
            }


        }
        #endregion   

    }


}
