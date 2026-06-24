using System.Diagnostics;

namespace Flare.Tools.Md3SpecParser.Cli;

/// <summary>Minimal git helper for staging and committing generated files.</summary>
public static class Git
{
    /// <summary>
    /// Stages and commits exactly <paramref name="paths"/> with <paramref name="message"/>.
    /// Returns success and combined git output.
    /// </summary>
    public static (bool Ok, string Output) Commit(IReadOnlyList<string> paths, string message, string workingDir)
    {
        if (paths.Count == 0) return (false, "no files to commit");

        var add = Run(workingDir, new[] { "add", "--" }.Concat(paths).ToArray());
        if (add.Exit != 0) return (false, add.Output);

        var commitArgs = new List<string> { "commit", "-m", message, "--" };
        commitArgs.AddRange(paths);
        var commit = Run(workingDir, commitArgs.ToArray());
        return (commit.Exit == 0, commit.Output);
    }

    private static (int Exit, string Output) Run(string workingDir, string[] args)
    {
        var psi = new ProcessStartInfo("git")
        {
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        foreach (var a in args) psi.ArgumentList.Add(a);

        try
        {
            using var process = Process.Start(psi)
                ?? throw new InvalidOperationException("Failed to start git.");
            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return (process.ExitCode, (stdout + stderr).Trim());
        }
        catch (Exception ex)
        {
            return (-1, ex.Message);
        }
    }
}
