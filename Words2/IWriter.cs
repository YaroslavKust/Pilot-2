﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Words2
{
    interface IWriter
    {
        void Write(string message);
        void WriteError(string textError);
    }
}
