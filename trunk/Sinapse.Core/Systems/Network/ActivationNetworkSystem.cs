using System;
using System.Collections.Generic;
using System.Text;

using AForge.Neuro;

namespace Sinapse.Core.Systems
{
    public class ActivationNetworkSystem : NetworkSystem, ISerializableObject<ActivationNetworkSystem>
    {
        private SerializableObject<ActivationNetworkSystem> serializableObject;



        public ActivationNetworkSystem(IActivationFunction function, int inputsCount, params int[] neuronsCount)
        {
            Network = new ActivationNetwork(function, inputsCount, neuronsCount);
        }

        public ActivationNetworkSystem()
        {
        }





        public new ActivationNetwork Network
        {
            get { return network as ActivationNetwork; }
            set { network = value; }
        }

        public override string Type
        {
            get { return "Activation Network"; }
        }

        public string Function
        {
            get { return this.Network[0][0].ActivationFunction.GetType().Name; }
        }







        public override object[][] Compute(params object[][] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override object[] Test(object[] input, object[] desiredOutput, out double[] rawOutput, out double[] deviation)
        {
            throw new Exception("The method or operation is not implemented.");
        }




        #region ISerializableObject<ActivationNetworkSystem> Members


        public string FileName
        {
            get { return serializableObject.FileName; }
            set { serializableObject.FileName = value; }
        }

        public string FilePath
        {
            get { return serializableObject.FilePath; }
            set { serializableObject.FilePath = value; }
        }

        public string DefaultExtension
        {
            get { return "sann"; }
        }

        public string FullPath
        {
            get { return serializableObject.FullPath; }
        }


        public bool Save(string path)
        {
            bool success = serializableObject.Save(path);
            if (success) this.HasChanges = false;
            return success;
        }

        public bool Save()
        {
            bool success = serializableObject.Save();
            if (success) this.HasChanges = false;
            return success;
        }

        public static ActivationNetworkSystem Open(string path)
        {
            return SerializableObject<ActivationNetworkSystem>.Open(path);
        }


        public event EventHandler FileChanged
        {
            add { serializableObject.FileChanged += value; }
            remove { serializableObject.FileChanged -= value; }
        }
        #endregion
    }
}
