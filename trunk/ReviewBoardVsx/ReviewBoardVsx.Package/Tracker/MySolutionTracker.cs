using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
//using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;

namespace ReviewBoardVsx.Package.Tracker
{
    /// <summary>
    /// Ideas taken from:
    /// SolutionListener.cs
    /// Project.cs
    /// ProjectDocumentsListener.cs
    /// ProjectDocumentsChangeEventArgs.cs
    /// 
    /// See Also:
    /// http://msdn.microsoft.com/en-us/library/bb165701.aspx
    /// 
    /// What is w/ these classes?
    /// Microsoft.VisualStudio.Package.ProjectPackage.cs
    /// Microsoft.VisualStudio.Package.SolutionListenerForProjectEvents.cs
    /// ...ProjectDocumentsListener.cs
    /// </summary>
    class MySolutionTracker : SolutionListener
    {
        /// <summary>
        /// A list of all changed files in the solution and projects
        /// TODO:(pv) Determine *type* of change (Added, Modified, Removed, etc...)
        /// </summary>
        private readonly List<string> filesChanged = new List<string>();

        /// <summary>
        /// A read-only list of all changed files in the solution and projects
        /// TODO:(pv) Determine *type* of change (Added, Modified, Removed, etc...)
        /// </summary>
        public ReadOnlyCollection<string> FilesChanged { get { return filesChanged.AsReadOnly(); } }

        protected readonly MyPackage package;
        private readonly MyProjectTracker projectTracker;

        class MyProjectTracker : ProjectDocumentsListener
        {
            protected readonly MyPackage package;

            public MyProjectTracker(MyPackage package) : base(package)
            {
                this.package = package; 
            }

            public override int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
            {
                try
                {
                    package.TraceEnter("OnQueryAddFiles");
                    //AddFilesIfChanged(rgpProjects, rgFirstIndices, rgpszMkDocuments);
                    return VSConstants.S_OK;
                }
                finally
                {
                    package.TraceLeave("OnQueryAddFiles");
                }
            }

            public override int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
            {
                try
                {
                    package.TraceEnter("OnAfterAddFilesEx");
                    return VSConstants.S_OK;
                }
                finally
                {
                    package.TraceLeave("OnAfterAddFilesEx");
                }
            }

            public override int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
            {
                try
                {
                    package.TraceEnter("OnQueryRenameFiles");

                    string oldname, newname;
                    for (int i = 0; i < cFiles; i++)
                    {
                        oldname = rgszMkOldNames[i];
                        newname = rgszMkNewNames[i];
                        // TODO:(pv) Rename in filesChanged
                    }
                    return VSConstants.S_OK;
                }
                finally
                {
                    package.TraceLeave("OnQueryRenameFiles");
                }
            }

            public override int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
            {
                try
                {
                    package.TraceEnter("OnAfterRenameFiles");
                    return VSConstants.S_OK;
                }
                finally
                {
                    package.TraceLeave("OnAfterRenameFiles");
                }
            }

            public override int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
            {
                try
                {
                    package.TraceEnter("OnQueryRemoveFiles");
                    string name;
                    for (int i = 0; i < cFiles; i++)
                    {
                        name = rgpszMkDocuments[i];
                        // TODO:(pv) Remove from filesChanged
                    }
                    return VSConstants.S_OK;
                }
                finally
                {
                    package.TraceLeave("OnQueryRemoveFiles");
                }
            }

            public override int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
            {
                try
                {
                    package.TraceEnter("OnAfterRemoveFiles");
                    return VSConstants.S_OK;
                }
                finally
                {
                    package.TraceLeave("OnAfterRemoveFiles");
                }
            }

            public override int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults)
            {
                try
                {
                    package.TraceEnter("OnQueryAddDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    package.TraceLeave("OnQueryAddDirectories");
                }
            }

            public override int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
            {
                try
                {
                    package.TraceEnter("OnAfterAddDirectoriesEx");
                    return VSConstants.S_OK;
                }
                finally
                {
                    package.TraceLeave("OnAfterAddDirectoriesEx");
                }
            }

            public override int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
            {
                try
                {
                    package.TraceEnter("OnQueryRenameDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    package.TraceLeave("OnQueryRenameDirectories");
                }
            }

            public override int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
            {
                try
                {
                    package.TraceEnter("OnAfterRemoveDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    package.TraceLeave("OnAfterRemoveDirectories");
                }
            }

            public override int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
            {
                try
                {
                    package.TraceEnter("OnQueryRemoveDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    package.TraceLeave("OnQueryRemoveDirectories");
                }
            }

            public override int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
            {
                try
                {
                    package.TraceEnter("OnAfterRenameDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    package.TraceLeave("OnAfterRenameDirectories");
                }
            }

            public override int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
            {
                try
                {
                    package.TraceEnter("OnAfterSccStatusChanged");
                    return VSConstants.S_OK;
                }
                finally
                {
                    package.TraceLeave("OnAfterSccStatusChanged");
                }
            }

            /*
            private void GenerateEvents(
                IVsProject[] projects,
                int[] firstFiles,
                string[] mkDocuments,
                EventHandler<ProjectDocumentsChangeEventArgs> eventToGenerate,
                ProjectDocumentsChangeEventArgs e)
            {
                if (eventToGenerate == null)
                    return; // no event = nothing to do

                if (projects == null || firstFiles == null || mkDocuments == null)
                    throw new ArgumentNullException();
                if (projects.Length != firstFiles.Length)
                    throw new ArgumentException();

                // First find out which range of the array (if any) include the files that belong to this project
                int first = -1;
                int last = mkDocuments.Length - 1; // default to the last document
                for (int i = 0; i < projects.Length; ++i)
                {
                    if (first > -1)
                    {
                        // We get here if there is 1 or more project(s) after ours in the list
                        last = firstFiles[i] - 1;
                        break;
                    }
                    if (Object.ReferenceEquals(projects[i], this))
                        first = firstFiles[i];
                }
                if (last >= mkDocuments.Length)
                    throw new ArgumentException();
                // See if we have any documents
                if (first < 0)
                    return; // Nothing that belongs to this project

                // For each file, generate the event
                for (int i = first; i <= last; ++i)
                {
                    try
                    {
                        e.MkDocument = mkDocuments[i];
                        eventToGenerate(this, e);
                    }
                    catch (Exception error)
                    {
                        Debug.Fail(error.Message);
                    }
                }
            }
            */
        }

        public MySolutionTracker(MyPackage package) : base(package)
        {
            this.package = package;
            this.projectTracker = new MyProjectTracker(package);
        }

        public override void Initialize()
        {
            try
            {
                package.TraceEnter("Initialize");
                base.Initialize();
                projectTracker.Initialize();
            }
            finally
            {
                package.TraceLeave("Initialize");
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                package.TraceEnter("Dispose");
                projectTracker.Dispose();
                base.Dispose(disposing);
            }
            finally
            {
                package.TraceLeave("Dispose");
            }
        }

        public override int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            try
            {
                package.TraceEnter("OnAfterOpenSolution");
                lock (filesChanged)
                {
                    filesChanged.Clear();
                }
                // TODO:(pv) Start crawling the solution items
                return VSConstants.S_OK;
            }
            finally
            {
                package.TraceLeave("OnAfterOpenSolution");
            }
        }

        /// <summary>
        /// The project has been opened.
        /// </summary>
        /// <param name="hierarchy">Pointer to the IVsHierarchy interface of the project being loaded.</param>
        /// <param name="added"> true if the project is added to the solution after the solution is opened. false if the project is added to the solution while the solution is being opened.</param>
        /// <returns></returns>
        public override int OnAfterOpenProject(IVsHierarchy hierarchy, int added)
        {
            try
            {
                package.TraceEnter("OnAfterOpenProject");
                // TODO:(pv) Start crawling the project items
                return VSConstants.S_OK;
            }
            finally
            {
                package.TraceLeave("OnAfterOpenProject");
            }
        }

        public override int OnAfterRenameProject(IVsHierarchy hierarchy)
        {
            try
            {
                package.TraceEnter("OnAfterRenameProject");
                // TODO:(pv) Rename the project item
                return VSConstants.S_OK;
            }
            finally
            {
                package.TraceLeave("OnAfterRenameProject");
            }
        }

        /// <summary>
        /// The project is about to be closed.
        /// </summary>
        /// <param name="hierarchy">Pointer to the IVsHierarchy interface of the project being closed.</param>
        /// <param name="removed">true if the project was removed from the solution before the solution was closed. false if the project was removed from the solution while the solution was being closed.</param>
        /// <returns></returns>
        public override int OnBeforeCloseProject(IVsHierarchy hierarchy, int removed)
        {
            try
            {
                package.TraceEnter("OnBeforeCloseProject");
                // TODO:(pv) Remove the project items
                return VSConstants.S_OK;
            }
            finally
            {
                package.TraceLeave("OnBeforeCloseProject");
            }
        }

        public override int OnQueryCloseProject(IVsHierarchy hierarchy, int removing, ref int cancel)
        {
            try
            {
                package.TraceEnter("OnQueryCloseProject");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                package.TraceLeave("OnQueryCloseProject");
            }
        }

        public override int OnQueryCloseSolution(object pUnkReserved, ref int cancel)
        {
            try
            {
                package.TraceEnter("OnQueryCloseSolution");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                package.TraceLeave("OnQueryCloseSolution");
            }
        }

        public override int OnBeforeCloseSolution(object pUnkReserved)
        {
            try
            {
                package.TraceEnter("OnBeforeCloseSolution");
                lock (filesChanged)
                {
                    filesChanged.Clear();
                }
                return VSConstants.S_OK;
            }
            finally
            {
                package.TraceLeave("OnBeforeCloseSolution");
            }
        }

        public override int OnAfterCloseSolution(object reserved)
        {
            try
            {
                package.TraceEnter("OnAfterCloseSolution");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                package.TraceLeave("OnAfterCloseSolution");
            }
        }

        public override int OnAfterClosingChildren(IVsHierarchy hierarchy)
        {
            try
            {
                package.TraceEnter("OnAfterClosingChildren");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                package.TraceLeave("OnAfterClosingChildren");
            }
        }

        public override int OnAfterLoadProject(IVsHierarchy stubHierarchy, IVsHierarchy realHierarchy)
        {
            try
            {
                package.TraceEnter("OnAfterLoadProject");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                package.TraceLeave("OnAfterLoadProject");
            }
        }

        public override int OnAfterMergeSolution(object pUnkReserved)
        {
            try
            {
                package.TraceEnter("OnAfterMergeSolution");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                package.TraceLeave("OnAfterMergeSolution");
            }
        }

        public override int OnAfterOpeningChildren(IVsHierarchy hierarchy)
        {
            try
            {
                package.TraceEnter("OnAfterOpeningChildren");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                package.TraceLeave("OnAfterOpeningChildren");
            }
        }

        public override int OnBeforeClosingChildren(IVsHierarchy hierarchy)
        {
            try
            {
                package.TraceEnter("OnBeforeClosingChildren");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                package.TraceLeave("OnBeforeClosingChildren");
            }
        }

        public override int OnBeforeOpeningChildren(IVsHierarchy hierarchy)
        {
            try
            {
                package.TraceEnter("OnBeforeOpeningChildren");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                package.TraceLeave("OnBeforeOpeningChildren");
            }
        }

        public override int OnBeforeUnloadProject(IVsHierarchy realHierarchy, IVsHierarchy rtubHierarchy)
        {
            try
            {
                package.TraceEnter("OnBeforeUnloadProject");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                package.TraceLeave("OnBeforeUnloadProject");
            }
        }

        public override int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int cancel)
        {
            try
            {
                package.TraceEnter("OnQueryUnloadProject");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                package.TraceLeave("OnQueryUnloadProject");
            }
        }

        public override int OnAfterAsynchOpenProject(IVsHierarchy hierarchy, int added)
        {
            try
            {
                package.TraceEnter("OnAfterAsynchOpenProject");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                package.TraceLeave("OnAfterAsynchOpenProject");
            }
        }

        public override int OnAfterChangeProjectParent(IVsHierarchy hierarchy)
        {
            try
            {
                package.TraceEnter("OnAfterChangeProjectParent");
                // log this to see if needed
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                package.TraceLeave("OnAfterChangeProjectParent");
            }
        }

        public override int OnQueryChangeProjectParent(IVsHierarchy hierarchy, IVsHierarchy newParentHier, ref int cancel)
        {
            try
            {
                package.TraceEnter("OnQueryChangeProjectParent");
                // log this to see if needed
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                package.TraceLeave("OnQueryChangeProjectParent");
            }
        }
    }
}
