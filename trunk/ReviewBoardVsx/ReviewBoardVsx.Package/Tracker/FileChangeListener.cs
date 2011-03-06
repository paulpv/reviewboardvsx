using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;

namespace ReviewBoardVsx.Package.Tracker
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivsfilechangeevents(v=VS.90).aspx
    /// http://nativeclient-sdk.googlecode.com/svn-history/r502/trunk/src/third_party/Microsoft.VisualStudio.Project/FileChangeManager.cs
    /// http://www.koders.com/csharp/fid127BC2AFCC2D56826135983EBCA899BD8E2601AA.aspx
    /// </summary>
    public abstract class FileChangeListener : IVsFileChangeEvents, IDisposable
    {
        private IVsFileChangeEx fileChangeEx;
        private Dictionary<string, uint> eventCookies = new Dictionary<string, uint>();

        private bool isDisposed;
        private static volatile object Mutex = new object();

        public FileChangeListener(IServiceProvider serviceProvider)
        {
            fileChangeEx = serviceProvider.GetService(typeof(SVsFileChangeEx)) as IVsFileChangeEx;
            Debug.Assert(fileChangeEx != null, "Could not get the IVsFileChangeEx object from the services exposed by this project");
            if (fileChangeEx == null)
            {
                throw new InvalidOperationException();
            }
        }

        #region IVsFileChangeEvents Members

        public abstract int FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange);

        public int DirectoryChanged(string pszDirectory)
        {
            return VSConstants.S_OK; // This *File*ChangeListener class is not interested in *Directory* changes.
        }

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
                    if (disposing)
                    {
                        Clear();
                    }
                    isDisposed = true;
                }
            }
        }

        public void Subscribe(string filepath)
        {
            if (fileChangeEx != null && !String.IsNullOrEmpty(filepath))
            {
                uint cookie;
                lock (eventCookies)
                {
                    ErrorHandler.ThrowOnFailure(fileChangeEx.AdviseFileChange(filepath, (uint)(_VSFILECHANGEFLAGS.VSFILECHG_Size | _VSFILECHANGEFLAGS.VSFILECHG_Time), this, out cookie));
                    eventCookies.Add(filepath, cookie);
                }
            }
        }

        public void Ignore(string filepath, bool ignore)
        {
            if (fileChangeEx != null && !String.IsNullOrEmpty(filepath))
            {
                uint cookie;
                lock (eventCookies)
                {
                    if (eventCookies.TryGetValue(filepath, out cookie))
                    {
                        ErrorHandler.ThrowOnFailure(fileChangeEx.IgnoreFile(cookie, filepath, (ignore) ? 0 : 1));
                    }
                }
            }
        }

        public void Unsubscribe(string filepath)
        {
            if (fileChangeEx != null && !String.IsNullOrEmpty(filepath))
            {
                uint cookie;
                lock (eventCookies)
                {
                    if (eventCookies.TryGetValue(filepath, out cookie))
                    {
                        ErrorHandler.ThrowOnFailure(fileChangeEx.UnadviseFileChange(cookie));
                    }
                }
            }
        }

        public void Clear()
        {
            lock (eventCookies)
            {
                foreach (string filepath in eventCookies.Keys)
                {
                    Unsubscribe(filepath);
                }
                eventCookies.Clear();
            }
        }
    }
}
