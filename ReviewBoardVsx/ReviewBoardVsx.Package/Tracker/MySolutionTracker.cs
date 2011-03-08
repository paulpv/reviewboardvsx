using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
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
    public class MySolutionTracker : SolutionListener
    {
        /// <summary>
        /// A list of all changed files in the solution and projects.
        /// </summary>
        private readonly PostReview.SubmitItemCollection changes = new PostReview.SubmitItemCollection();

        public PostReview.SubmitItemReadOnlyCollection Changes { get { return changes.AsReadOnly(); } }

        private readonly Dictionary<string, string> mapItemProjects = new Dictionary<string,string>();

        public bool IsInitialSolutionCrawlFinished { get; private set; }
        public readonly BackgroundWorker BackgroundInitialSolutionCrawl;

        private readonly MyProjectTracker projectTracker;
        private readonly MyFileTracker fileTracker;

        #region MyProjectTracker

        /// <summary>
        /// Ideas came from:
        /// http://www.java2s.com/Open-Source/CSharp/Development/StyleCop/Microsoft/VisualStudio/Shell/Flavor/Project.cs.htm
        /// </summary>
        class MyProjectTracker : ProjectDocumentsListener
        {
            private MySolutionTracker solutionTracker;

            public MyProjectTracker(MySolutionTracker solutionTracker)
                : base(solutionTracker.ServiceProvider)
            {
                this.solutionTracker = solutionTracker;
            }

            public override int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
            {
                try
                {
                    MyLog.DebugEnter(this, "OnQueryAddFiles");
                    //AddFilesIfChanged(rgpProjects, rgFirstIndices, rgpszMkDocuments);
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyLog.DebugLeave(this, "OnQueryAddFiles");
                }
            }

            public override int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
            {
                try
                {
                    MyLog.DebugEnter(this, "OnAfterAddFilesEx");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyLog.DebugLeave(this, "OnAfterAddFilesEx");
                }
            }

            public override int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
            {
                try
                {
                    MyLog.DebugEnter(this, "OnQueryRenameFiles");

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
                    MyLog.DebugLeave(this, "OnQueryRenameFiles");
                }
            }

            public override int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
            {
                try
                {
                    MyLog.DebugEnter(this, "OnAfterRenameFiles");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyLog.DebugLeave(this, "OnAfterRenameFiles");
                }
            }

            public override int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
            {
                try
                {
                    MyLog.DebugEnter(this, "OnQueryRemoveFiles");
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
                    MyLog.DebugLeave(this, "OnQueryRemoveFiles");
                }
            }

            public override int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
            {
                try
                {
                    MyLog.DebugEnter(this, "OnAfterRemoveFiles");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyLog.DebugLeave(this, "OnAfterRemoveFiles");
                }
            }

            public override int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults)
            {
                try
                {
                    MyLog.DebugEnter(this, "OnQueryAddDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyLog.DebugLeave(this, "OnQueryAddDirectories");
                }
            }

            public override int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
            {
                try
                {
                    MyLog.DebugEnter(this, "OnAfterAddDirectoriesEx");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyLog.DebugLeave(this, "OnAfterAddDirectoriesEx");
                }
            }

            public override int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
            {
                try
                {
                    MyLog.DebugEnter(this, "OnQueryRenameDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyLog.DebugLeave(this, "OnQueryRenameDirectories");
                }
            }

            public override int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
            {
                try
                {
                    MyLog.DebugEnter(this, "OnAfterRemoveDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyLog.DebugLeave(this, "OnAfterRemoveDirectories");
                }
            }

            public override int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
            {
                try
                {
                    MyLog.DebugEnter(this, "OnQueryRemoveDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyLog.DebugLeave(this, "OnQueryRemoveDirectories");
                }
            }

            public override int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
            {
                try
                {
                    MyLog.DebugEnter(this, "OnAfterRenameDirectories");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyLog.DebugLeave(this, "OnAfterRenameDirectories");
                }
            }

            public override int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
            {
                try
                {
                    MyLog.DebugEnter(this, "OnAfterSccStatusChanged");
                    return VSConstants.S_OK;
                }
                finally
                {
                    MyLog.DebugLeave(this, "OnAfterSccStatusChanged");
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

        #endregion MyProjectTracker

        #region MyFileTracker

        public class MyFileTracker : FileChangeListener
        {
            private MySolutionTracker solutionTracker;

            public MyFileTracker(MySolutionTracker solutionTracker)
                : base(solutionTracker.ServiceProvider)
            {
                this.solutionTracker = solutionTracker;
            }

            public override int FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange)
            {
                try
                {
                    MyLog.DebugEnter(this, "FilesChanged(" + cChanges + ", " + rgpszFile + ", " + rggrfChange + ")");

                    foreach(string filepath in rgpszFile)
                    {
                        solutionTracker.AddFilePathIfChanged(filepath);
                    }

                    return VSConstants.S_OK;
                }
                finally
                {
                    MyLog.DebugLeave(this, "FilesChanged(" + cChanges + ", " + rgpszFile + ", " + rggrfChange + ")");
                }
            }
        }

        #endregion MyFileTracker

        public MySolutionTracker(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.projectTracker = new MyProjectTracker(this);
            this.fileTracker = new MyFileTracker(this);

            BackgroundInitialSolutionCrawl = new BackgroundWorker();
            BackgroundInitialSolutionCrawl.WorkerReportsProgress = true;
            BackgroundInitialSolutionCrawl.DoWork += backgroundInitialSolutionCrawl_DoWork;
        }

        public override void Initialize()
        {
            try
            {
                MyLog.DebugEnter(this, "Initialize()");
                // TODO:(pv) Should I do this at SolutionOpen?
                base.Initialize();
                projectTracker.Initialize();
            }
            finally
            {
                MyLog.DebugLeave(this, "Initialize()");
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                MyLog.DebugEnter(this, "Dispose(" + disposing + ")");
                // TODO:(pv) Should I do this at SolutionClose?
                fileTracker.Dispose();
                projectTracker.Dispose();
                base.Dispose(disposing);
            }
            finally
            {
                MyLog.DebugLeave(this, "Dispose(" + disposing + ")");
            }
        }

        void backgroundInitialSolutionCrawl_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                MyLog.DebugEnter(this, "EnumHierarchyItems");
                EnumHierarchyItems((IVsHierarchy)Solution, VSConstants.VSITEMID_ROOT, 0, true, true);
            }
            finally
            {
                MyLog.DebugLeave(this, "EnumHierarchyItems");
            }

            IsInitialSolutionCrawlFinished = true;
        }

        #region event handlers

        public override int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterOpenSolution(" + pUnkReserved + ", " + fNewSolution + ")");

                // TODO:(pv) What if another crawl is already running?
                // TODO:(pv) Is this the best place for this?
                lock (changes)
                {
                    changes.Clear();
                }

                IsInitialSolutionCrawlFinished = false;

                BackgroundInitialSolutionCrawl.RunWorkerAsync();

                return VSConstants.S_OK;
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterOpenSolution(" + pUnkReserved + ", " + fNewSolution + ")");
            }
        }

        public override int OnQueryCloseSolution(object pUnkReserved, ref int cancel)
        {
            try
            {
                MyLog.DebugEnter(this, "OnQueryCloseSolution(" + pUnkReserved + ", " + cancel + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyLog.DebugLeave(this, "OnQueryCloseSolution(" + pUnkReserved + ", " + cancel + ")");
            }
        }

        public override int OnBeforeCloseSolution(object pUnkReserved)
        {
            try
            {
                MyLog.DebugEnter(this, "OnBeforeCloseSolution(" + pUnkReserved + ")");

                BackgroundInitialSolutionCrawl.CancelAsync();

                IsInitialSolutionCrawlFinished = false;

                lock (changes)
                {
                    changes.Clear();
                }

                return VSConstants.S_OK;
            }
            finally
            {
                MyLog.DebugLeave(this, "OnBeforeCloseSolution(" + pUnkReserved + ")");
            }
        }

        public override int OnAfterCloseSolution(object reserved)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterCloseSolution(" + reserved + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterCloseSolution(" + reserved + ")");
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
                MyLog.DebugEnter(this, "OnAfterOpenProject(" + hierarchy + ", " + added + ")");
                // TODO:(pv) Start crawling the project items
                return VSConstants.S_OK;
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterOpenProject(" + hierarchy + ", " + added + ")");
            }
        }

        public override int OnAfterRenameProject(IVsHierarchy hierarchy)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterRenameProject(" + hierarchy + ")");
                // TODO:(pv) Rename the project item
                return VSConstants.S_OK;
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterRenameProject(" + hierarchy + ")");
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
                MyLog.DebugEnter(this, "OnBeforeCloseProject(" + hierarchy + ", " + removed + ")");
                // TODO:(pv) Remove the project items
                return VSConstants.S_OK;
            }
            finally
            {
                MyLog.DebugLeave(this, "OnBeforeCloseProject(" + hierarchy + ", " + removed + ")");
            }
        }

        public override int OnQueryCloseProject(IVsHierarchy hierarchy, int removing, ref int cancel)
        {
            try
            {
                MyLog.DebugEnter(this, "OnQueryCloseProject(" + hierarchy + ", " + removing + ", " + cancel + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyLog.DebugLeave(this, "OnQueryCloseProject(" + hierarchy + ", " + removing + ", " + cancel + ")");
            }
        }

        public override int OnAfterClosingChildren(IVsHierarchy hierarchy)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterClosingChildren(" + hierarchy + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterClosingChildren(" + hierarchy + ")");
            }
        }

        public override int OnAfterLoadProject(IVsHierarchy stubHierarchy, IVsHierarchy realHierarchy)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterLoadProject(" + stubHierarchy + ", " + realHierarchy + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterLoadProject(" + stubHierarchy + ", " + realHierarchy + ")");
            }
        }

        public override int OnAfterMergeSolution(object pUnkReserved)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterMergeSolution(" + pUnkReserved + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterMergeSolution(" + pUnkReserved + ")");
            }
        }

        public override int OnAfterOpeningChildren(IVsHierarchy hierarchy)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterOpeningChildren(" + hierarchy + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterOpeningChildren(" + hierarchy + ")");
            }
        }

        public override int OnBeforeClosingChildren(IVsHierarchy hierarchy)
        {
            try
            {
                MyLog.DebugEnter(this, "OnBeforeClosingChildren(" + hierarchy + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyLog.DebugLeave(this, "OnBeforeClosingChildren(" + hierarchy + ")");
            }
        }

        public override int OnBeforeOpeningChildren(IVsHierarchy hierarchy)
        {
            try
            {
                MyLog.DebugEnter(this, "OnBeforeOpeningChildren(" + hierarchy + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyLog.DebugLeave(this, "OnBeforeOpeningChildren(" + hierarchy + ")");
            }
        }

        public override int OnBeforeUnloadProject(IVsHierarchy realHierarchy, IVsHierarchy stubHierarchy)
        {
            try
            {
                MyLog.DebugEnter(this, "OnBeforeUnloadProject(" + realHierarchy + ", " + stubHierarchy + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyLog.DebugLeave(this, "OnBeforeUnloadProject(" + realHierarchy + ", " + stubHierarchy + ")");
            }
        }

        public override int OnQueryUnloadProject(IVsHierarchy realHierarchy, ref int cancel)
        {
            try
            {
                MyLog.DebugEnter(this, "OnQueryUnloadProject(" + realHierarchy + ", " + cancel + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyLog.DebugLeave(this, "OnQueryUnloadProject(" + realHierarchy + ", " + cancel + ")");
            }
        }

        public override int OnAfterAsynchOpenProject(IVsHierarchy hierarchy, int added)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterAsynchOpenProject(" + hierarchy + ", " + added + ")");
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterAsynchOpenProject(" + hierarchy + ", " + added + ")");
            }
        }

        public override int OnAfterChangeProjectParent(IVsHierarchy hierarchy)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterChangeProjectParent(" + hierarchy + ")");
                // log this to see if needed
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterChangeProjectParent(" + hierarchy + ")");
            }
        }

        public override int OnQueryChangeProjectParent(IVsHierarchy hierarchy, IVsHierarchy newParentHier, ref int cancel)
        {
            try
            {
                MyLog.DebugEnter(this, "OnQueryChangeProjectParent(" + hierarchy + ", " + newParentHier + ", " + cancel + ")");
                // log this to see if needed
                return VSConstants.S_OK; // We are not interested in this one
            }
            finally
            {
                MyLog.DebugLeave(this, "OnQueryChangeProjectParent(" + hierarchy + ", " + newParentHier + ", " + cancel + ")");
            }
        }

        #endregion

        #region crawler method(s)

        /// <summary>
        /// Code almost 100% taken from VS SDK Example: SolutionHierarchyTraversal
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="itemid"></param>
        /// <param name="recursionLevel"></param>
        /// <param name="hierIsSolution"></param>
        /// <param name="visibleNodesOnly"></param>
        /// <param name="changes"></param>
        /// <returns>true if the caller should continue, false if the caller should stop</returns>
        private bool EnumHierarchyItems(IVsHierarchy hierarchy, uint itemid, int recursionLevel, bool hierIsSolution, bool visibleNodesOnly)//, PostReview.SubmitItemCollection changes)
        {
            if (BackgroundInitialSolutionCrawl != null && BackgroundInitialSolutionCrawl.CancellationPending)
            {
                return false;
            }

            int hr;
            IntPtr nestedHierarchyObj;
            uint nestedItemId;
            Guid hierGuid = typeof(IVsHierarchy).GUID;

            // Check first if this node has a nested hierarchy. If so, then there really are two 
            // identities for this node: 1. hierarchy/itemid 2. nestedHierarchy/nestedItemId.
            // We will recurse and call EnumHierarchyItems which will display this node using
            // the inner nestedHierarchy/nestedItemId identity.
            hr = hierarchy.GetNestedHierarchy(itemid, ref hierGuid, out nestedHierarchyObj, out nestedItemId);
            if (VSConstants.S_OK == hr && IntPtr.Zero != nestedHierarchyObj)
            {
                IVsHierarchy nestedHierarchy = Marshal.GetObjectForIUnknown(nestedHierarchyObj) as IVsHierarchy;
                Marshal.Release(nestedHierarchyObj);    // we are responsible to release the refcount on the out IntPtr parameter
                if (nestedHierarchy != null)
                {
                    // Display name and type of the node in the Output Window
                    EnumHierarchyItems(nestedHierarchy, nestedItemId, recursionLevel, false, visibleNodesOnly);
                }
            }
            else
            {
                object pVar;

                // Display name and type of the node in the Output Window
                ProcessNode(hierarchy, itemid, recursionLevel);

                recursionLevel++;

                // Get the first child node of the current hierarchy being walked
                // NOTE: to work around a bug with the Solution implementation of VSHPROPID_FirstChild,
                // we keep track of the recursion level. If we are asking for the first child under
                // the Solution, we use VSHPROPID_FirstVisibleChild instead of _FirstChild. 
                // In VS 2005 and earlier, the Solution improperly enumerates all nested projects
                // in the Solution (at any depth) as if they are immediate children of the Solution.
                // Its implementation _FirstVisibleChild is correct however, and given that there is
                // not a feature to hide a SolutionFolder or a Project, thus _FirstVisibleChild is 
                // expected to return the identical results as _FirstChild.
                hr = hierarchy.GetProperty(itemid,
                    ((visibleNodesOnly || (hierIsSolution && recursionLevel == 1) ?
                        (int)__VSHPROPID.VSHPROPID_FirstVisibleChild : (int)__VSHPROPID.VSHPROPID_FirstChild)),
                    out pVar);
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
                if (VSConstants.S_OK == hr)
                {
                    // We are using Depth first search so at each level we recurse to check if the node has any children
                    // and then look for siblings.
                    uint childId = MyPackage.GetItemId(pVar);
                    while (childId != VSConstants.VSITEMID_NIL)
                    {
                        if (!EnumHierarchyItems(hierarchy, childId, recursionLevel, false, visibleNodesOnly))
                        {
                            break;
                        }

                        // NOTE: to work around a bug with the Solution implementation of VSHPROPID_NextSibling,
                        // we keep track of the recursion level. If we are asking for the next sibling under
                        // the Solution, we use VSHPROPID_NextVisibleSibling instead of _NextSibling. 
                        // In VS 2005 and earlier, the Solution improperly enumerates all nested projects
                        // in the Solution (at any depth) as if they are immediate children of the Solution.
                        // Its implementation   _NextVisibleSibling is correct however, and given that there is
                        // not a feature to hide a SolutionFolder or a Project, thus _NextVisibleSibling is 
                        // expected to return the identical results as _NextSibling.
                        hr = hierarchy.GetProperty(childId,
                            ((visibleNodesOnly || (hierIsSolution && recursionLevel == 1)) ?
                                (int)__VSHPROPID.VSHPROPID_NextVisibleSibling : (int)__VSHPROPID.VSHPROPID_NextSibling),
                            out pVar);
                        if (VSConstants.S_OK == hr)
                        {
                            childId = MyPackage.GetItemId(pVar);
                        }
                        else
                        {
                            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
                            break;
                        }
                    }
                }
            }

            return (BackgroundInitialSolutionCrawl == null || !BackgroundInitialSolutionCrawl.CancellationPending);
        }

        private void ProcessNode(IVsHierarchy hierarchy, uint itemId, int recursionLevel)
        {
            try
            {
                MyLog.DebugEnter(this, "ProcessNode(hierarchy, " + itemId + ", " + recursionLevel + ")");

                int hr;

                Object oRootName;
                hr = hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_Name, out oRootName);
                if (ErrorHandler.Failed(hr))
                {
                    MyPackage.OutputGeneral("ERROR: Could not get root name of item #" + itemId);
                    ErrorHandler.ThrowOnFailure(hr);
                }
                string rootName = oRootName as string;
                if (String.IsNullOrEmpty(rootName))
                {
                    rootName = Resources.RootUnknown;
                }
                Debug.WriteLine("rootName=" + rootName);

                string itemName;
                hr = hierarchy.GetCanonicalName(itemId, out itemName);
                if (ErrorHandler.Failed(hr))
                {
                    switch (hr)
                    {
                        case VSConstants.E_NOTIMPL:
                            // ignore; Nothing to do if we cannot get the file name, but below logic can handle null/empty name...
                            itemName = null;
                            break;
                        default:
                            MyPackage.OutputGeneral("ERROR: Could not get canonical name of item #" + itemId);
                            ErrorHandler.ThrowOnFailure(hr);
                            break;
                    }
                }
                Debug.WriteLine("itemName=\"" + itemName + "\"");

#if DEBUG && false
                if (BackgroundWorker != null && !String.IsNullOrEmpty(itemName))
                {
                    // Temporary until we call AddFilePathIfChanged after we find out what the item type is
                    BackgroundWorker.ReportProgress(0, itemName);
                }
#endif

                Guid guidTypeNode;
                hr = hierarchy.GetGuidProperty(itemId, (int)__VSHPROPID.VSHPROPID_TypeGuid, out guidTypeNode);
                if (ErrorHandler.Failed(hr))
                {
                    switch (hr)
                    {
                        case VSConstants.E_NOTIMPL:
                            Debug.WriteLine("Guid property E_NOTIMPL for item #" + itemId + " \"" + itemName + "\"; assuming virtual/reference item and ignoring");
                            // ignore; Below logic can handle Guid.Empty
                            guidTypeNode = Guid.Empty;
                            break;
                        case VSConstants.DISP_E_MEMBERNOTFOUND:
                            Debug.WriteLine("Guid property DISP_E_MEMBERNOTFOUND for item #" + itemId + " \"" + itemName + "\"; assuming reference item and ignoring");
                            guidTypeNode = Guid.Empty;
                            break;
                        default:
                            MyPackage.OutputGeneral("ERROR: Could not get type guid of item #" + itemId + " \"" + itemName + "\"");
                            ErrorHandler.ThrowOnFailure(hr);
                            break;
                    }
                }
                Debug.WriteLine("guidTypeNode=" + guidTypeNode);

                //
                // Intentionally ordered from most commonly expected to least commonly expected...
                //
                if (Guid.Equals(guidTypeNode, VSConstants.GUID_ItemType_PhysicalFile))
                {
                    AddFilePathIfChanged(itemName, rootName);
                }
                else if (itemId == VSConstants.VSITEMID_ROOT)
                {
                    IVsProject project = hierarchy as IVsProject;
                    if (project != null)
                    {
                        string projectFile;
                        project.GetMkDocument(VSConstants.VSITEMID_ROOT, out projectFile);
                        AddFilePathIfChanged(projectFile, rootName);
                    }
                    else
                    {
                        IVsSolution solution = hierarchy as IVsSolution;
                        if (solution != null)
                        {
                            rootName = Resources.RootSolution;

                            string solutionDirectory, solutionFile, solutionUserOptions;
                            ErrorHandler.ThrowOnFailure(solution.GetSolutionInfo(out solutionDirectory, out solutionFile, out solutionUserOptions));
                            AddFilePathIfChanged(solutionFile, rootName);
                        }
                        else
                        {
                            MyPackage.OutputGeneral("ERROR: itemid==VSITEMID_ROOT, but hierarchy is neither Solution or Project");
                            ErrorHandler.ThrowOnFailure(VSConstants.E_UNEXPECTED);
                        }
                    }
                }
#if DEBUG
                else if (Guid.Equals(guidTypeNode, VSConstants.GUID_ItemType_PhysicalFolder))
                {
                    Debug.WriteLine("ignoring GUID_ItemType_PhysicalFolder");
                    // future enumeration will handle any individual subitems in this folder...
                }
                else if (Guid.Equals(guidTypeNode, VSConstants.GUID_ItemType_VirtualFolder))
                {
                    Debug.WriteLine("ignoring GUID_ItemType_VirtualFolder");
                    // future enumeration will handle any individual subitems in this virtual folder...
                }
                else if (Guid.Equals(guidTypeNode, VSConstants.GUID_ItemType_SubProject))
                {
                    Debug.WriteLine("ignoring GUID_ItemType_SubProject");
                    // future enumeration will handle any individual subitems in this sub project...
                }
                else if (Guid.Equals(guidTypeNode, Guid.Empty))
                {
                    Debug.WriteLine("ignoring itemName=" + itemName + "; guidTypeNode == Guid.Empty");
                    // future enumeration will handle any individual subitems in this item...
                }
                else
                {
                    MyPackage.OutputGeneral("ERROR: Unhandled node item/type itemName=" + itemName + ", guidTypeNode=" + guidTypeNode);
                    ErrorHandler.ThrowOnFailure(VSConstants.E_UNEXPECTED);
                }
#endif
            }
            finally
            {
                MyLog.DebugLeave(this, "ProcessNode(hierarchy, " + itemId + ", " + recursionLevel + ")");
            }
        }

        public void AddFilePathIfChanged(string filePath)
        {
            string project;
            if (!mapItemProjects.TryGetValue(filePath.ToLower(), out project))
            {
                project = Resources.RootUnknown;
            }
            AddFilePathIfChanged(filePath, project);
        }

        public void AddFilePathIfChanged(string filePath, string project)
        {
            try
            {
                MyLog.DebugEnter(this, "AddFilePathIfChanged(\"" + filePath + "\", \"" + project + "\")");

                filePath = MyUtils.GetCasedFilePath(filePath);
                if (String.IsNullOrEmpty(filePath))
                {
                    // If we got this far then the *Solution* says there is a file.
                    // However, the file can be an external/symbolic link/reference, not an actual file.
                    // Ignore this situation and just continue the enumeration.
                    return;
                }

                if (BackgroundInitialSolutionCrawl != null && BackgroundInitialSolutionCrawl.IsBusy)
                {
                    // Percent is currently always 0, since our progress is indeterminate
                    // TODO:(pv) Find some way to determine # of nodes in tree *before* processing
                    //      Maybe do a quick first pass w/ no post-review?
                    // NOTE:(pv) I did once have the debugger halt here complaining invalid state that the form is not active
                    BackgroundInitialSolutionCrawl.ReportProgress(0, filePath);
                }

                string diff;
                PostReview.DiffType diffType = PostReview.DiffFile(filePath, out diff);

                switch (diffType)
                {
                    case PostReview.DiffType.Added:
                    case PostReview.DiffType.Changed:
                    case PostReview.DiffType.Modified:
                        PostReview.SubmitItem change = new PostReview.SubmitItem(filePath, project, diffType, diff);
                        changes.Add(change);
                        break;
                    case PostReview.DiffType.External:
                        // TODO:(pv) Even add External items?
                        // Doesn't make much sense, since they won't diff during post-review.exe submit.
                        // Maybe could add them and group them at bottom as disabled.
                        // This also doesn't make sense, since it assumes post-review has ability to add files to SCM.
                        break;
                    case PostReview.DiffType.Normal:
                        // TODO:(pv) Even add normal items?
                        // Doesn't make much sense, since they won't diff during post-review.exe submit.
                        // Maybe could add them and group them at bottom as disabled.
                        // This also doesn't make sense, since it assumes post-review has ability to add files to SCM.
                        break;
                    default:
                        string message = String.Format("Unhandled DiffType={0}", diffType);
                        throw new ArgumentOutOfRangeException("diffType", message);
                }

                filePath = filePath.ToLower();

                // *Always* track the file for *future* changes, even if there are no *current* changes.
                // This is a no-op if the path is already being tracked.
                fileTracker.Subscribe(filePath, true);

                // Map the file path to a project so that future file changes can find out what project the file is in given just the file path.
                // This will overwrite any existing value...which seems fine for now.
                mapItemProjects[filePath] = project;
            }
            finally
            {
                MyLog.DebugLeave(this, "AddFilePathIfChanged(\"" + filePath + "\", \"" + project + "\")");
            }
        }

        #endregion crawler method(s)
    }
}
