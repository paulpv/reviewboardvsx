using System;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;

namespace ReviewBoardVsx.Package.Tracker
{
    /// <summary>
    /// This class keeps track of files and directories being added, renamed [moved?], or removed from a Project.
    /// 
    /// Ideas came from Ankh ProjectTracker and:
    /// http://www.java2s.com/Open-Source/CSharp/Development/StyleCop/Microsoft/VisualStudio/Package/ProjectDocumentsListener.cs.htm
    /// http://www.java2s.com/Open-Source/CSharp/Development/StyleCop/Microsoft/VisualStudio/Shell/Flavor/Project.cs.htm
    /// http://www.java2s.com/Open-Source/CSharp/Development/StyleCop/Microsoft/VisualStudio/Shell/Flavor/ProjectDocumentsChangeEventsArgs.cs.htm
    /// </summary>
    [CLSCompliant(false)]
    public abstract class ProjectDocumentsListener : IVsTrackProjectDocumentsEvents2, IDisposable
    {
        public class ProjectItemAddRemoveEventArgs : EventArgs
        {
            public IVsProject Project { get; private set; }
            public String Path { get; private set; }

            public ProjectItemAddRemoveEventArgs(IVsProject project, string path)
            {
                Project = project;
                Path = path;
            }
        }

        public class ProjectItemRenameEventArgs : EventArgs
        {
            public IVsProject Project { get; private set; }
            public String PathOld { get; private set; }
            public String PathNew { get; private set; }

            public ProjectItemRenameEventArgs(IVsProject project, string pathOld, string pathNew)
            {
                Project = project;
                PathOld = pathOld;
                PathNew = pathNew;
            }
        }

        //public delegate void EventHandler<ProjectDocumentsChangeEventArgs>(object sender, ProjectDocumentsChangeEventArgs e);

        public event EventHandler<ProjectItemAddRemoveEventArgs> FileAdded;
        public event EventHandler<ProjectItemRenameEventArgs> FileRenamed;
        public event EventHandler<ProjectItemAddRemoveEventArgs> FileRemoved;
        public event EventHandler<ProjectItemAddRemoveEventArgs> DirectoryAdded;
        public event EventHandler<ProjectItemRenameEventArgs> DirectoryRenamed;
        public event EventHandler<ProjectItemAddRemoveEventArgs> DirectoryRemoved;

        public IServiceProvider ServiceProvider { get; private set; }

        private IVsTrackProjectDocuments2 projectDocumentTracker2;

        private uint eventsCookie = (uint)ShellConstants.VSCOOKIE_NIL;
        private bool isDisposed;
        private static volatile object Mutex = new object();

        protected ProjectDocumentsListener(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            projectDocumentTracker2 = serviceProvider.GetService(typeof(SVsTrackProjectDocuments)) as IVsTrackProjectDocuments2;
            Debug.Assert(projectDocumentTracker2 != null, "Could not get the IVsTrackProjectDocuments2 object from the services exposed by this project");
            if (projectDocumentTracker2 == null)
            {
                throw new InvalidOperationException();
            }
        }

        public void Initialize()
        {
            if (projectDocumentTracker2 != null && eventsCookie == (uint)ShellConstants.VSCOOKIE_NIL)
            {
                ErrorHandler.ThrowOnFailure(projectDocumentTracker2.AdviseTrackProjectDocumentsEvents(this, out eventsCookie));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                lock (Mutex)
                {
                    if (disposing && projectDocumentTracker2 != null && eventsCookie != (uint)ShellConstants.VSCOOKIE_NIL)
                    {
                        ErrorHandler.ThrowOnFailure(projectDocumentTracker2.UnadviseTrackProjectDocumentsEvents((uint)eventsCookie));
                        eventsCookie = (uint)ShellConstants.VSCOOKIE_NIL;
                    }
                    isDisposed = true;
                }
            }
        }

        #region IVsTrackProjectDocumentsEvents2 methods

        public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
        {
            try
            {
                MyLog.DebugEnter(this, "OnQueryAddFiles");
                return VSConstants.S_OK; // ignore
            }
            finally
            {
                MyLog.DebugLeave(this, "OnQueryAddFiles");
            }
        }

        public int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterAddFilesEx");
                GenerateAddRemoveEvents(rgpProjects, rgFirstIndices, rgpszMkDocuments, FileAdded);
                return VSConstants.S_OK;
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterAddFilesEx");
            }
        }

        public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
        {
            try
            {
                MyLog.DebugEnter(this, "OnQueryRenameFiles");
                return VSConstants.S_OK; // ignore
            }
            finally
            {
                MyLog.DebugLeave(this, "OnQueryRenameFiles");
            }
        }

        public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterRenameFiles");
                GenerateRenameEvents(rgpProjects, rgFirstIndices, rgszMkOldNames, rgszMkNewNames, FileRenamed);
                return VSConstants.S_OK;
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterRenameFiles");
            }
        }

        public int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
        {
            try
            {
                MyLog.DebugEnter(this, "OnQueryRemoveFiles");
                return VSConstants.S_OK; // ignore
            }
            finally
            {
                MyLog.DebugLeave(this, "OnQueryRemoveFiles");
            }
        }

        public int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterRemoveFiles");
                GenerateAddRemoveEvents(rgpProjects, rgFirstIndices, rgpszMkDocuments, FileRemoved);
                return VSConstants.S_OK;
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterRemoveFiles");
            }
        }

        public int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults)
        {
            try
            {
                MyLog.DebugEnter(this, "OnQueryAddDirectories");
                return VSConstants.S_OK; // ignore
            }
            finally
            {
                MyLog.DebugLeave(this, "OnQueryAddDirectories");
            }
        }

        public int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterAddDirectoriesEx");
                GenerateAddRemoveEvents(rgpProjects, rgFirstIndices, rgpszMkDocuments, DirectoryAdded);
                return VSConstants.S_OK;
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterAddDirectoriesEx");
            }
        }

        public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            try
            {
                MyLog.DebugEnter(this, "OnQueryRenameDirectories");
                return VSConstants.S_OK; // ignore
            }
            finally
            {
                MyLog.DebugLeave(this, "OnQueryRenameDirectories");
            }
        }

        public int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterRenameDirectories");
                GenerateRenameEvents(rgpProjects, rgFirstIndices, rgszMkOldNames, rgszMkNewNames, DirectoryRenamed);
                return VSConstants.S_OK;
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterRenameDirectories");
            }
        }

        public int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
        {
            try
            {
                MyLog.DebugEnter(this, "OnQueryRemoveDirectories");
                return VSConstants.S_OK; // ignore
            }
            finally
            {
                MyLog.DebugLeave(this, "OnQueryRemoveDirectories");
            }
        }

        public int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterRemoveDirectories");
                GenerateAddRemoveEvents(rgpProjects, rgFirstIndices, rgpszMkDocuments, DirectoryRemoved);
                return VSConstants.S_OK;
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterRemoveDirectories");
            }
        }

        public int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
        {
            try
            {
                MyLog.DebugEnter(this, "OnAfterSccStatusChanged");
                return VSConstants.S_OK; // ignore
            }
            finally
            {
                MyLog.DebugLeave(this, "OnAfterSccStatusChanged");
            }
        }

        #endregion IVsTrackProjectDocumentsEvents2 methods

        private void GenerateAddRemoveEvents(
            IVsProject[] projects,
            int[] firstPaths,
            string[] paths,
            EventHandler<ProjectItemAddRemoveEventArgs> eventToGenerate)
        {
            if (eventToGenerate == null)
                return; // no event = nothing to do

            if (projects == null || firstPaths == null || paths == null)
            {
                throw new ArgumentNullException();
            }

            if (projects.Length != firstPaths.Length)
            {
                throw new ArgumentException();
            }

            int cProjects = projects.Length;
            int cPaths = paths.Length;

            IVsProject project;
            string origin;
            string pathNew;

            int iPath = 0;
            for (int iProject = 0; (iProject < cProjects) && (iPath < cPaths); iProject++)
            {
                int iLastPathThisProject = (iProject < cProjects - 1) ? firstPaths[iProject + 1] : cPaths;

                for (; iPath < iLastPathThisProject; iPath++)
                {
                    origin = null;
                    pathNew = paths[iPath];// SvnTools.GetNormalizedFullPath(rgszMkNewNames[iFile]);

                    // if project == null then Solution, else Project
                    project = projects[iProject];

                    try
                    {
                        eventToGenerate(this, new ProjectItemAddRemoveEventArgs(project, pathNew));
                    }
                    catch (Exception error)
                    {
                        Debug.Fail(error.Message);
                    }
                }
            }
        }

        private void GenerateRenameEvents(
            IVsProject[] projects,
            int[] firstPaths,
            string[] pathsOld,
            string[] pathsNew,
            EventHandler<ProjectItemRenameEventArgs> eventToGenerate)
        {
            if (eventToGenerate == null)
                return; // no event = nothing to do

            if (projects == null || firstPaths == null || pathsOld == null || pathsNew == null)
            {
                throw new ArgumentNullException();
            }

            if (projects.Length != firstPaths.Length)
            {
                throw new ArgumentException();
            }

            if (pathsOld.Length != pathsNew.Length)
            {
                throw new ArgumentException();
            }

            int cProjects = projects.Length;
            int cPaths = pathsOld.Length;

            // TODO: C++ projects do not send directory renames; but do send OnAfterRenameFile() events
            //       for all files (one at a time). We should detect that case here and fix up this dirt!

            IVsProject project;
            string pathOld;
            string pathNew;

            int iPath = 0;
            for (int iProject = 0; (iProject < cProjects) && (iPath < cPaths); iProject++)
            {
                int iLastPathThisProject = (iProject < cProjects - 1) ? firstPaths[iProject + 1] : cPaths;

                for (; iPath < iLastPathThisProject; iPath++)
                {
                    pathOld = pathsOld[iPath];// SvnTools.GetNormalizedFullPath(pathsOld[iFile]);
                    pathNew = pathsNew[iPath];// SvnTools.GetNormalizedFullPath(pathsNew[iFile]);

                    if (pathOld == pathNew)
                        continue;

                    // if project == null then Solution, else Project
                    project = projects[iProject];

                    try
                    {
                        eventToGenerate(this, new ProjectItemRenameEventArgs(project, pathOld, pathNew));
                    }
                    catch (Exception error)
                    {
                        Debug.Fail(error.Message);
                    }
                }
            }
        }
    }
}