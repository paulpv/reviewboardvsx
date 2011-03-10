using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace ReviewBoardVsx.Package.Tracker
{
    /// <summary>
    /// </summary>
    class MyProjectTracker : ProjectDocumentsListener
    {
        private MySolutionTracker solutionTracker;

        public MyProjectTracker(MySolutionTracker solutionTracker)
            : base(solutionTracker.ServiceProvider)
        {
            this.solutionTracker = solutionTracker;

            this.FileAdded += MyProjectTracker_FileAdded;
            this.FileRenamed += MyProjectTracker_FileRenamed;
            this.FileRemoved += MyProjectTracker_FileRemoved;
        }

        void MyProjectTracker_FileAdded(object sender, ProjectDocumentsListener.ProjectItemsAddRemoveEventArgs e)
        {
            string projectName = solutionTracker.GetProjectName(e.Project);
            foreach (string item in e.Items)
            {
                solutionTracker.AddFilePathIfChanged(item, projectName);
            }
        }

        void MyProjectTracker_FileRenamed(object sender, ProjectDocumentsListener.ProjectItemsRenameEventArgs e)
        {
            string projectName = solutionTracker.GetProjectName(e.Project);
            foreach (ProjectItemsRenameEventArgs.RenamedItem item in e.Items)
            {
                //solutionTracker.RenameFilePath(item, projectName);
            }
        }

        void MyProjectTracker_FileRemoved(object sender, ProjectDocumentsListener.ProjectItemsAddRemoveEventArgs e)
        {
            string projectName = solutionTracker.GetProjectName(e.Project);
            foreach (string item in e.Items)
            {
                //solutionTracker.RemoveFilePath(item, projectName);
            }
        }
    }
}
