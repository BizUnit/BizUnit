//---------------------------------------------------------------------
// File: TestStage.cs
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

/// <summary>The TestStage enumeration is used to specify the stage of a BizUnit 
/// test case, each instance of a test step must be assigned to a single stage.
/// </summary>
public enum TestStage
{
    ///<summary>The setup stage of a test, typically used for initialising the 
    /// conditions ready to run the test</summary>
    Setup = 0,
    ///<summary>The execution stage of a test. typically this stage of the test 
    /// represents the actual scenario being tested</summary>
    Execution = 1,
    ///<summary>The cleanup stage is always executed, typically this stage should 
    /// contain the neccessary steps to revert the platform to its state prior 
    /// to the test runnning</summary>
    Cleanup = 2
}
