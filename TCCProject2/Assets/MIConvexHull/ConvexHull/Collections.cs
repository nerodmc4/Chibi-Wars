namespace MIConvexHull
{
    using System;
    using System.Collections.Generic;

    sealed class VertexBuffer
    {
        VertexWrap[] items;
        int count;
        int capacity;

        public int Count { get { return count; } }

        public VertexWrap this[int i]{
            get { return items[i]; }
        }

       void EnsureCapacity(){
            if (count + 1 > capacity)
            {
                if (capacity == 0) capacity = 4;
                else capacity = 2 * capacity;
                Array.Resize(ref items, capacity);
            }
        }

        public void Add(VertexWrap item){
            EnsureCapacity();
            items[count++] = item;
        }

		public void Clear(){
            count = 0;
        }
    }
        
    sealed class FaceList
    {
        ConvexFaceInternal first, last;

        public ConvexFaceInternal First { get { return first; } }

        void AddFirst(ConvexFaceInternal face)
        {
            face.InList = true;
            this.first.Previous = face;
            face.Next = this.first;
            this.first = face;
        }

       public void Add(ConvexFaceInternal face)
        {
            if (face.InList){
                if (this.first.VerticesBeyond.Count < face.VerticesBeyond.Count){
                    Remove(face);
                    AddFirst(face);
                }
                return;
            }

            face.InList = true;

            if (first != null && first.VerticesBeyond.Count < face.VerticesBeyond.Count)
            {
                this.first.Previous = face;
                face.Next = this.first;
                this.first = face;
            }
            else
            {
                if (this.last != null)
                {
                    this.last.Next = face;
                }
                face.Previous = this.last;
                this.last = face;
                if (this.first == null)
                {
                    this.first = face;
                }
            }
        }

        public void Remove(ConvexFaceInternal face)
        {
            if (!face.InList) return;

            face.InList = false;

            if (face.Previous != null)
            {
                face.Previous.Next = face.Next;
            }
            else if (face.Previous == null)
            {
                this.first = face.Next;
            }

            if (face.Next != null)
            {
                face.Next.Previous = face.Previous;
            }
            else if (face.Next == null)
            {
                this.last = face.Previous;
            }

            face.Next = null;
            face.Previous = null;
        }
    }

    sealed class ConnectorList
    {
        FaceConnector first, last;

        public FaceConnector First { get { return first; } }

        void AddFirst(FaceConnector connector)
        {
            this.first.Previous = connector;
            connector.Next = this.first;
            this.first = connector;
        }

        public void Add(FaceConnector element)
        {
            if (this.last != null)
            {
                this.last.Next = element;
            }
            element.Previous = this.last;
            this.last = element;
            if (this.first == null)
            {
                this.first = element;
            }
        }

        public void Remove(FaceConnector connector)
        {
            if (connector.Previous != null)
            {
                connector.Previous.Next = connector.Next;
            }
            else if (connector.Previous == null)
            {
                this.first = connector.Next;
            }

            if (connector.Next != null)
            {
                connector.Next.Previous = connector.Previous;
            }
            else if (connector.Next == null)
            {
                this.last = connector.Previous;
            }

            connector.Next = null;
            connector.Previous = null;
        }
    }
}
