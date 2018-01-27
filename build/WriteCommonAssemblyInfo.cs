using System;  
using Microsoft.Build.Framework;  
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class WriteCommonAssemblyInfo: ITask  
{  
    //When implementing the ITask interface, it is necessary to  
    //implement a BuildEngine property of type  
    //Microsoft.Build.Framework.IBuildEngine. This is done for  
    //you if you derive from the Task class.  
    private IBuildEngine buildEngine;  
    public IBuildEngine BuildEngine  
    {  
        get  
        {  
            return buildEngine;  
        }  
        set  
        {  
            buildEngine = value;  
        }  
        }  

    // When implementing the ITask interface, it is necessary to  
    // implement a HostObject property of type Object.  
    // This is done for you if you derive from the Task class.  
    private ITaskHost hostObject;  
    public ITaskHost HostObject  
    {  
        get  
        {  
            return hostObject;  
        }  

        set  
        {  
            hostObject = value;  
        }  
    } 

    public bool Execute()  
    {  
        return true;  
    }  
}  