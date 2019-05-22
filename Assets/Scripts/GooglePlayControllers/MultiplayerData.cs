using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MultiplayerData
{
    private const int Header = 422001;

    public byte[] WritePlayerPosition(float x, float y)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryWriter w = new BinaryWriter(memStream);
        w.Write(Header);
        /*w.Write((byte)mParticipantIdX.Length);
        w.Write(mParticipantIdX.ToCharArray());
        int x;
        for (x = 0; x < mBoard.Length; x++)
        {
            w.Write(mBoard[x]);
        }
        w.Write(mBlockDescs.Count);
        foreach (BlockDesc b in mBlockDescs)
        {
            w.Write(b.mark);
            w.Write(b.position.x);
            w.Write(b.position.y);
            w.Write(b.position.z);
            w.Write(b.rotation.x);
            w.Write(b.rotation.y);
            w.Write(b.rotation.z);
            w.Write(b.rotation.w);
        }*/
        w.Close();
        byte[] buf = memStream.GetBuffer();
        memStream.Close();
        return buf;
    }

    private void ReadFromBytes(byte[] b)
    {
        BinaryReader r = new BinaryReader(new MemoryStream(b));
        int header = r.ReadInt32();
        if (header != Header)
        {
            // we don't know how to parse this version; user has to upgrade game
            return;
        }
    }
}