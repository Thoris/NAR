rem cd..
cls
rem Runner.exe -r fitnesse.fitserver.FitServer,fit.dll -v  D:\Thoris\Projetos\CI\CISample\CI.Sample.Fit.Tests\bin\Release\CI.Sample.Fit.Tests.dll localhost 8888  HelloWorld
rem cd tools
rem pause
rem Runner.exe -r fitnesse.fitserver.FitServer,fit.dll -v  D:\Thoris\Projetos\CI\CISample\CI.Sample.Fit.Tests\bin\Release\CI.Sample.Fit.Tests.dll localhost 8888 /TestAdd
rem Runner.exe -r fitSharp.Slim.Service.Runner,fitSharp.dll -c app.xml 8881

cls
 Runner.exe -r fitnesse.fitserver.FitServer,fit.dll -v  D:\Thoris\Projetos\CI\CISample\CI.Sample.Fit.Tests\bin\Release\CI.Sample.Fit.Tests.dll localhost 8888 HelloWorld

rem java -cp fitnesse.jar fitnesse.runner.TestTunner localhost 8888 HelloWorld

pause