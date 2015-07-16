//---------------------------------------------------------------------
// File: TestGroupPhase.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2015, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

/// <summary>The TestGroupPhase enumeration is used to specify the setup and teardown phases of a group of test cases. For example when using the NUnit attributes: [TestFixtureSetUp] or [TestFixtureTearDown].
/// </summary>
public enum TestGroupPhase
{
    ///<summary>The start of the test group setup</summary>
    TestGroupSetup,
    ///<summary>The end of the test group setup</summary>
    TestGroupTearDown,
    ///<summary>Undefined</summary>
    Unknown
};
