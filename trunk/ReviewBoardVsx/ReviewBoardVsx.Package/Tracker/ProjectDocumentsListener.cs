using System;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;

namespace ReviewBoardVsx.Package.Tracker
{
    /// <summary>
    /// Most of this class was shamelessly copied from:
    /// http://www.java2s.com/Open-Source/CSharp/Development/StyleCop/Microsoft/VisualStudio/Package/ProjectDocumentsListener.cs.htm
    /// </summary>
    [CLSCompliant(false)]
    public abstract class ProjectDocumentsListener : IVsTrackProjectDocumentsEvents2, IDisposable
    {
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

        #region IVsTrackProjectDocumentsEvents2 Members

        public abstract int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags);
        public abstract int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags);
        public abstract int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags);
        public abstract int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags);
        public abstract int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags);
        public abstract int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags);
        public abstract int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus);
        public abstract int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults);
        public abstract int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults);
        public abstract int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults);
        public abstract int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults);
        public abstract int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults);
        public abstract int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults);

        #endregion

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

        public void Initialize()
        {
            if (projectDocumentTracker2 != null && eventsCookie == (uint)ShellConstants.VSCOOKIE_NIL)
            {
                ErrorHandler.ThrowOnFailure(projectDocumentTracker2.AdviseTrackProjectDocumentsEvents(this, out eventsCookie));
            }
        }
    }
}