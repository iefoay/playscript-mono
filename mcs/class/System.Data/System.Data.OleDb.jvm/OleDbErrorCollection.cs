//
// System.Data.OleDb.OleDbErrorCollection
//
// Authors:
//	Konstantin Triger <kostat@mainsoft.com>
//	Boris Kirzner <borisk@mainsoft.com>
//	
// (C) 2005 Mainsoft Corporation (http://www.mainsoft.com)
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


namespace System.Data.OleDb
{


    using System.Collections;
	using java.sql;
	using System.Data.Common;
	using System.Data.ProviderBase;

	[Serializable]
    public sealed class OleDbErrorCollection : AbstractDbErrorCollection
    {
		internal OleDbErrorCollection(SQLException e, AbstractDBConnection connection) : base(e, connection) {
		}
        /**
         * Gets the error at the specified index.
         *
         * @param index of the error
         * @return Error on specified index
         */
        public OleDbError this[int index]
        {
            get
            {
                return (OleDbError)GetDbItem(index);
            }
        }

#if NET_2_0
		public void CopyTo (OleDbError [] array, int index)
		{
			base.CopyTo (array, index);
		}

#endif

		protected override AbstractDbError CreateDbError(SQLException e, AbstractDBConnection connection) {
			return new OleDbError(e, connection);
		}

    }
}