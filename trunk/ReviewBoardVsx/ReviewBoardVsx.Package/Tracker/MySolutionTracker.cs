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
    /// Project.cs
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

        /// <summary>
        /// Ideas came from:
        /// http://www.java2s.com/Open-Source/CSharp/Development/StyleCop/Microsoft/VisualStudio/Shell/Flavor/Project.cs.htm
        /// </summary>
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
                    MyPackage.TraceEnter(this, "OnQueryAddFiles");
                    //AddFilesIfChanged(rgpProjects, rgFirstIndices, rgpszMkDocuments);
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyPackage.TraceLeave(this, "OnQueryAddFiles");
                }
            }

            public override int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
            {
                try
                {
                    MyPackage.TraceEnter(this, "OnAfterAddFilesEx");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyPackage.TraceLeave(this, "OnAfterAddFilesEx");
                }
            }

            public override int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
            {
                try
                {
                    MyPackage.TraceEnter(this, "OnQueryRenameFiles");

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
                    MyPackage.TraceLeave(this, "OnQueryRenameFiles");
                }
            }

            public override int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
            {
                try
                {
                    MyPackage.TraceEnter(this, "OnAfterRenameFiles");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyPackage.TraceLeave(this, "OnAfterRenameFiles");
                }
            }

            public override int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
            {
                try
                {
                    MyPackage.TraceEnter(this, "OnQueryRemoveFiles");
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
                    MyPackage.TraceLeave(this, "OnQueryRemoveFiles");
                }
            }

            public override int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
            {
                try
                {
                    MyPackage.TraceEnter(this, "OnAfterRemoveFiles");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyPackage.TraceLeave(this, "OnAfterRemoveFiles");
                }
            }

            public override int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults)
            {
                try
                {
                    MyPackage.TraceEnter(this, "OnQueryAddDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyPackage.TraceLeave(this, "OnQueryAddDirectories");
                }
            }

            public override int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
            {
                try
                {
                    MyPackage.TraceEnter(this, "OnAfterAddDirectoriesEx");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyPackage.TraceLeave(this, "OnAfterAddDirectoriesEx");
                }
            }

            public override int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
            {
                try
                {
                    MyPackage.TraceEnter(this, "OnQueryRenameDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyPackage.TraceLeave(this, "OnQueryRenameDirectories");
                }
            }

            public override int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
            {
                try
                {
                    MyPackage.TraceEnter(this, "OnAfterRemoveDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyPackage.TraceLeave(this, "OnAfterRemoveDirectories");
                }
            }

            public override int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
            {
                try
                {
                    MyPackage.TraceEnter(this, "OnQueryRemoveDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyPackage.TraceLeave(this, "OnQueryRemoveDirectories");
                }
            }

            public override int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
            {
                try
                {
                    MyPackage.TraceEnter(this, "OnAfterRenameDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyPackage.TraceLeave(this, "OnAfterRenameDirectories");
                }
            }

            public override int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
            {
                try
                {
                    MyPackage.TraceEnter(this, "OnAfterSccStatusChanged");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyPackage.TraceLeave(this, "OnAfterSccStatusChanged");
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
                MyPackage.TraceEnter(this, "Initialize()");
                base.Initialize();
                projectTracker.Initialize();
            }
            finally
            {
                MyPackage.TraceLeave(this, "Initialize()");
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                MyPackage.TraceEnter(this, "Dispose(" + disposing + ")");
                projectTracker.Dispose();
                base.Dispose(disposing);
            }
            finally
            {
                MyPackage.TraceLeave(this, "Dispose(" + disposing + ")");
            }
        }

        public override int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnAfterOpenSolution(" + pUnkReserved + ", " + fNewSolution + ")");
                lock (filesChanged)
                {
                    filesChanged.Clear();
                }

                Debug.WriteLine("TODO:(pv) Start crawling the solution items");

                return VSConstants.S_OK;
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnAfterOpenSolution(" + pUnkReserved + ", " + fNewSolution + ")");
            }
        }

        /// <summary>
        /// The project has been opened.
        /// </summary>
        /// <param name="hierarchy">Pointer to the IVsHierarchy interface of the project being loaded.</param>
        /// <param name="added">1 if the project is added to the solution after the solution is opened. 0 if the project is added to the solution while the solution is being opened.</param>
        /// <returns></returns>
        public override int OnAfterOpenProject(IVsHierarchy hierarchy, int added)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnAfterOpenProject(" + hierarchy + ", " + added + ")");
                // TODO:(pv) Start crawling the project items
                return VSConstants.S_OK;
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnAfterOpenProject(" + hierarchy + ", " + added + ")");
            }
        }

        public override int OnAfterRenameProject(IVsHierarchy hierarchy)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnAfterRenameProject(" + hierarchy + ")");
                // TODO:(pv) Rename the project item
                return VSConstants.S_OK;
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnAfterRenameProject(" + hierarchy + ")");
            }
        }

        /// <summary>
        /// The project is about to be closed.
        /// </summary>
        /// <param name="hierarchy">Pointer to the IVsHierarchy interface of the project being closed.</param>
        /// <param name="removed">1 if the project was removed from the solution before the solution was closed. 0 if the project was removed from the solution while the solution was being closed.</param>
        /// <returns></returns>
        public override int OnBeforeCloseProject(IVsHierarchy hierarchy, int removed)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnBeforeCloseProject(" + hierarchy + ", " + removed + ")");
                // TODO:(pv) Remove the project items
                return VSConstants.S_OK;
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnBeforeCloseProject(" + hierarchy + ", " + removed + ")");
            }
        }

        public override int OnQueryCloseProject(IVsHierarchy hierarchy, int removing, ref int cancel)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnQueryCloseProject(" + hierarchy + ", " + removing + ", " + cancel + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnQueryCloseProject(" + hierarchy + ", " + removing + ", " + cancel + ")");
            }
        }

        public override int OnQueryCloseSolution(object pUnkReserved, ref int cancel)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnQueryCloseSolution(" + pUnkReserved + ", " + cancel + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnQueryCloseSolution(" + pUnkReserved + ", " + cancel + ")");
            }
        }

        public override int OnBeforeCloseSolution(object pUnkReserved)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnBeforeCloseSolution(" + pUnkReserved + ")");
                lock (filesChanged)
                {
                    filesChanged.Clear();
                }
                return VSConstants.S_OK;
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnBeforeCloseSolution(" + pUnkReserved + ")");
            }
        }

        public override int OnAfterCloseSolution(object reserved)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnAfterCloseSolution(" + reserved + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnAfterCloseSolution(" + reserved + ")");
            }
        }

        public override int OnAfterClosingChildren(IVsHierarchy hierarchy)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnAfterClosingChildren(" + hierarchy + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnAfterClosingChildren(" + hierarchy + ")");
            }
        }

        public override int OnAfterLoadProject(IVsHierarchy stubHierarchy, IVsHierarchy realHierarchy)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnAfterLoadProject(" + stubHierarchy + ", " + realHierarchy + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnAfterLoadProject(" + stubHierarchy + ", " + realHierarchy + ")");
            }
        }

        public override int OnAfterMergeSolution(object pUnkReserved)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnAfterMergeSolution(" + pUnkReserved + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnAfterMergeSolution(" + pUnkReserved + ")");
            }
        }

        public override int OnAfterOpeningChildren(IVsHierarchy hierarchy)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnAfterOpeningChildren(" + hierarchy + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnAfterOpeningChildren(" + hierarchy + ")");
            }
        }

        public override int OnBeforeClosingChildren(IVsHierarchy hierarchy)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnBeforeClosingChildren(" + hierarchy + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnBeforeClosingChildren(" + hierarchy + ")");
            }
        }

        public override int OnBeforeOpeningChildren(IVsHierarchy hierarchy)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnBeforeOpeningChildren(" + hierarchy + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnBeforeOpeningChildren(" + hierarchy + ")");
            }
        }

        public override int OnBeforeUnloadProject(IVsHierarchy realHierarchy, IVsHierarchy stubHierarchy)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnBeforeUnloadProject(" + realHierarchy + ", " + stubHierarchy + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnBeforeUnloadProject(" + realHierarchy + ", " + stubHierarchy + ")");
            }
        }

        public override int OnQueryUnloadProject(IVsHierarchy realHierarchy, ref int cancel)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnQueryUnloadProject(" + realHierarchy + ", " + cancel + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnQueryUnloadProject(" + realHierarchy + ", " + cancel + ")");
            }
        }

        public override int OnAfterAsynchOpenProject(IVsHierarchy hierarchy, int added)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnAfterAsynchOpenProject(" + hierarchy + ", " + added + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnAfterAsynchOpenProject(" + hierarchy + ", " + added + ")");
            }
        }

        public override int OnAfterChangeProjectParent(IVsHierarchy hierarchy)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnAfterChangeProjectParent(" + hierarchy + ")");
                // log this to see if needed
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnAfterChangeProjectParent(" + hierarchy + ")");
            }
        }

        public override int OnQueryChangeProjectParent(IVsHierarchy hierarchy, IVsHierarchy newParentHier, ref int cancel)
        {
            try
            {
                MyPackage.TraceEnter(this, "OnQueryChangeProjectParent(" + hierarchy + ", " + newParentHier + ", " + cancel + ")");
                // log this to see if needed
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyPackage.TraceLeave(this, "OnQueryChangeProjectParent(" + hierarchy + ", " + newParentHier + ", " + cancel + ")");
            }
        }
    }
}
