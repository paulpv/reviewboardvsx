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

        void MyProjectTracker_FileAdded(object sender, ProjectDocumentsListener.ProjectItemAddRemoveEventArgs e)
        {
            //solutionTracker.AddFilePathIfChanged(e.Path);
        }

        void MyProjectTracker_FileRenamed(object sender, ProjectDocumentsListener.ProjectItemRenameEventArgs e)
        {
        }

        void MyProjectTracker_FileRemoved(object sender, ProjectDocumentsListener.ProjectItemAddRemoveEventArgs e)
        {
        }
    }
}
