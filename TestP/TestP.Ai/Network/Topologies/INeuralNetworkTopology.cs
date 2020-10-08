using Keras.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestP.Ai.Network
{
    public interface INeuralNetworkTopology 
    {
        BaseModel CreateBaseModel();
        void CompileBaseModel(BaseModel model);
    }
}
