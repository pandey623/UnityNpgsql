// created on 1/7/2003 at 20:48

// Npgsql.NpgsqlDescribe.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.

using System;
using System.IO;
using System.Text;

namespace UnityNpgsql
{
    /// <summary>
    /// This is the base class for NpgsqlDescribeStatement and NpgsqlDescribePortal.
    /// </summary>
    ///
    internal abstract class NpgsqlDescribe : ClientMessage
    {
        protected enum DescribeTypeCode : byte
        {
            Statement = ASCIIBytes.S,
            Portal = ASCIIBytes.P
        }

        private readonly DescribeTypeCode _whatToDescribe;
        private readonly byte[] _bPortalName;
        private readonly int _messageLength;

        protected NpgsqlDescribe(DescribeTypeCode whatToDescribe, String portalName)
        {
            _whatToDescribe = whatToDescribe;

            _bPortalName = BackendEncoding.UTF8Encoding.GetBytes(portalName);

            _messageLength = 4 + 1 + _bPortalName.Length + 1;
        }

        public override void WriteToStream(Stream outputStream)
        {
            outputStream
                .WriteBytes((byte)FrontEndMessageCode.Describe)
                .WriteInt32(_messageLength)
                .WriteBytes((byte)_whatToDescribe)
                .WriteBytesNullTerminated(_bPortalName);
        }
    }

    /// <summary>
    /// This class represents the Statement Describe message sent to PostgreSQL
    /// server.
    /// </summary>
    ///
    internal sealed class NpgsqlDescribeStatement : NpgsqlDescribe
    {
        public NpgsqlDescribeStatement(String statementName)
        : base(DescribeTypeCode.Statement, statementName)
        {}
    }

    /// <summary>
    /// This class represents the Portal Describe message sent to PostgreSQL
    /// server.
    /// </summary>
    ///
    internal sealed class NpgsqlDescribePortal : NpgsqlDescribe
    {
        public NpgsqlDescribePortal(String portalName)
        : base(DescribeTypeCode.Portal, portalName)
        {}
    }
}
