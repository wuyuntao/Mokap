using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mokap.Bvh
{
    class MotionWriter : Disposable
    {
        private string filename;

        private TextWriter textWriter;

        public MotionWriter(string filename)
        {
            this.filename = filename;
            this.textWriter = new StreamWriter(filename);
        }

        protected override void DisposeManaged()
        {
            SafeDispose(ref this.textWriter);

            base.DisposeManaged();
        }

        public void Write()
        {
            throw new NotImplementedException();
        }
    }
}
