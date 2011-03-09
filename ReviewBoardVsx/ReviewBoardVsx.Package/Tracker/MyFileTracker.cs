using Microsoft.VisualStudio;

namespace ReviewBoardVsx.Package.Tracker
{
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

                foreach (string filepath in rgpszFile)
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
}
