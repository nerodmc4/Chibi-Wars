namespace MIConvexHull
{
    using System.Collections.Generic;

    class ObjectManager
    {
        readonly int Dimension;

        Stack<ConvexFaceInternal> RecycledFaceStack;
        Stack<FaceConnector> ConnectorStack;
        Stack<VertexBuffer> EmptyBufferStack;
        Stack<DeferredFace> DeferredFaceStack;

        public void DepositFace(ConvexFaceInternal face)
        {
            for (int i = 0; i < Dimension; i++)
            {
                face.AdjacentFaces[i] = null;
            }
            RecycledFaceStack.Push(face);
        }

        public ConvexFaceInternal GetFace()
        {
            return RecycledFaceStack.Count != 0
                    ? RecycledFaceStack.Pop()
                    : new ConvexFaceInternal(Dimension, GetVertexBuffer());
        }

        public void DepositConnector(FaceConnector connector)
        {
            ConnectorStack.Push(connector);
        }

        public FaceConnector GetConnector()
        {
            return ConnectorStack.Count != 0
                    ? ConnectorStack.Pop()
                    : new FaceConnector(Dimension);
        }

        public void DepositVertexBuffer(VertexBuffer buffer)
        {
            buffer.Clear();
            EmptyBufferStack.Push(buffer);
        }

        public VertexBuffer GetVertexBuffer()
        {
            return EmptyBufferStack.Count != 0 ? EmptyBufferStack.Pop() : new VertexBuffer();
        }

        public void DepositDeferredFace(DeferredFace face)
        {
            DeferredFaceStack.Push(face);
        }

        public DeferredFace GetDeferredFace()
        {
            return DeferredFaceStack.Count != 0 ? DeferredFaceStack.Pop() : new DeferredFace();
        }
                
        public ObjectManager(int dimension)
        {
            this.Dimension = dimension;

            RecycledFaceStack = new Stack<ConvexFaceInternal>();
            ConnectorStack = new Stack<FaceConnector>();
            EmptyBufferStack = new Stack<VertexBuffer>();
            DeferredFaceStack = new Stack<DeferredFace>();
        }
    }
}
