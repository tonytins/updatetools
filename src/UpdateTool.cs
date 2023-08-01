namespace TonyBark.UpdateDotNetTools;

static class UpdateTool
{
    public static void TryUpdate(string toolName)
    {
        try
        {
            ToolUpdate(toolName);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    static void ToolUpdate(string tool)
    {
        Console.WriteLine(Command("dotnet", $"tool update {tool} -g"));
    }

    public static IEnumerable<string> Parse(string command)
    {
        Console.WriteLine(command);

        var stringReader = new StringReader(command);
        var line = stringReader.ReadLine();
        while (line != null)
        {
            if (line.Contains("-------------------------"))
                break;

            line = stringReader.ReadLine();
        }

        line = stringReader.ReadLine();
        var regex = new Regex(@"(\S+)\s+", RegexOptions.Compiled);
        while (!string.IsNullOrWhiteSpace(line))
        {
            var match = regex.Match(line);
            yield return match.Groups[1].Value;
            line = stringReader.ReadLine();
        }
    }

    public static string Command(string fileName, string argument, string workingDirectory = null)
    {
        Console.WriteLine($"{fileName} {argument}");
        if (string.IsNullOrEmpty(workingDirectory))
        {
            workingDirectory = Environment.CurrentDirectory;
        }

        Console.WriteLine($"FileName={fileName}");
        Console.WriteLine($"Argument={argument}");
        Console.WriteLine($"WorkingDirectory={workingDirectory}");

        var process = new Process
        {
            StartInfo =
                {
                    FileName = fileName,
                    Arguments = argument,
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false, 
                    //RedirectStandardInput = true, 
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    //StandardOutputEncoding = Encoding.GetEncoding("GBK") //Encoding.UTF8
                    //Encoding.GetEncoding("GBK");
                }
        };

        process.Start(); //启动程序

        const string breakLine = "\r\n";
        string output = "";
        process.OutputDataReceived += (sender, args) =>
        {
            output += args.Data + breakLine;
        };

        process.ErrorDataReceived += (sender, args) =>
        {
            output += args.Data + breakLine;
        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        // If the application does not exit, then ReadToEnd will wait, that is, the current process will wait for the process to exit
        ////Get output information
        //string output = p.StandardOutput.ReadToEnd();
        //output += p.StandardError.ReadToEnd();

        // 等待程序执行完退出进程
        process.WaitForExit(TimeSpan.FromMinutes(1).Milliseconds);

        // 第二次等待解决 OutputDataReceived 调用的坑
        // 请看官方代码 https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.outputdatareceived?view=netcore-3.1
        process.WaitForExit();
        process.Close();

        return output + breakLine;
    }
}