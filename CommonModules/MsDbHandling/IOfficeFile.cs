#region [[[ COPYRIGHT_NOTICE ]]]
/*
 * ============================================================================
 * 
 * Copyright(c) 2011 Allright reserved.
 * 
 * 이 소스코드는, 상업적 용도를 포함하여 어떠한 목적으로든 자유롭게 사용 및
 * 수정, 배포 할 수 있습니다.
 * 
 * 이 소스코드는 "있는 그대로(AS-IS)"제공되며, 저작자는 어떠한 보증도 하지
 * 않습니다. 사용으로 인한 책임은 전적으로 사용자에게 있습니다.
 *
 * ----------------------------------------------------------------------------
 *
 * Author: GreenB
 * http://blog.greenmaru.com
 *
 * Latest update: 2011-10-12
 * 
 * ============================================================================
 */
#endregion

#region USING_STATEMENTS
using System;
#endregion  // USING_STATEMENTS


namespace MsDbHandler
{
    /// <summary>
    /// MS Office interop개체를 캡슐화 한 개체. 오피스 파일을 조작하기 위한 기능을 제공합니다.
    /// </summary>
    public interface IOfficeFile : IDisposable
    {
        /// <summary>
        /// 오피스 파일의 이름.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// 변경된 모든 사항을 오피스 파일에 저장합니다.
        /// </summary>
        void Save();

        /// <summary>
        /// 변경된 모든 사항을 다른 이름으로 저장합니다.
        /// </summary>
        /// <param name="fileName">저장할 파일의 이름.</param>
        void SaveAs(string fileName);
    }
}
