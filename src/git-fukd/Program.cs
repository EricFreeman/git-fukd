﻿using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;

namespace git_fukd
{
    class Program
    {
        static void Main(string[] args)
        {
            var changes = new Dictionary<string, int>();

            using (var repo = new Repository(@"A:\home\Projects\fuck"))
            {
                var commits = repo.Commits;
                Commit lastCommit = null;

                foreach (var commit in commits)
                {
                    var tree = commit.Tree;

                    if (lastCommit == null)
                    {
                        lastCommit = commit;
                        continue;
                    }

                    var parentCommitTree = lastCommit.Tree;
                    lastCommit = commit;

                    repo.Diff
                        .Compare<TreeChanges>(parentCommitTree, tree)
                        .ToList()
                        .ForEach(commitChanges =>
                        {
                            var key = commitChanges.Path;

                            if (changes.ContainsKey(key))
                            {
                                var value = changes[key] + 1;
                                changes[key] = value;
                            }
                            else
                            {
                                changes[key] = 1;
                            }
                        });
                }
            }

            Console.ReadLine();
        }
    }
}