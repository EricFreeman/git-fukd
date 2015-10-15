using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace git_fukd
{
    class Program
    {
        static void Main(string[] args)
        {
            var changes = new Dictionary<string, int>();
            var files = new List<string>();
            var spinner = new ConsoleSpiner();
            Console.Write("Working....");

            var directory = Directory.GetCurrentDirectory();
            directory = "C:\\home\\projects\\git-fukd";
            using (var repo = new Repository(directory))
            {
                var commits = repo.Commits;
                Commit lastCommit = null;

                var start = repo.Tags.First().Target.Sha;
                var end = repo.Tags.Last().Target.Sha;

                var isLooking = false;
                var isLast = false;


                foreach (var commit in commits)
                {
                    spinner.Turn();

                    var tree = commit.Tree;

                    if (lastCommit == null)
                    {
                        lastCommit = commit;
                        continue;
                    }

                    var parentCommitTree = lastCommit.Tree;
                    lastCommit = commit;

                    if (commit.Sha == start || commit.Sha == end)
                    {
                        isLooking = !isLooking;
                        isLast = !isLooking;
                    }

                    if (isLooking || isLast)
                    {
                        if (isLast)
                        {
                            parentCommitTree = null;
                        }

                        var c = repo.Diff
                           .Compare<TreeChanges>(parentCommitTree, tree)
                           .ToList();

                           c.ForEach(commitChanges =>
                           {
                               var file = commitChanges.Path;
                               if (file.EndsWith(".sql") && !files.Contains(file))
                               {
                                   files.Add(file);
                               }
                           });
                    }

                    if (isLast)
                    {
                        break;
                    }
            }

            Console.WriteLine();

            foreach (var change in files)
            {
                Console.WriteLine(change);
            }
        }
    }
}