// ----------------------------------------------------------------------------
// <copyright file="CustomTypes.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#pragma warning disable 1587
/// \file
/// <summary>Sets up support for Unity-specific types. Can be a blueprint how to register your own Custom Types for sending.</summary>
#pragma warning restore 1587


using ExitGames.Client.Photon;
using System;
using UnityEngine;

/// <summary>
/// Internally used class, containing de/serialization methods for various Unity-specific classes.
/// Adding those to the Photon serialization protocol allows you to send them in events, etc.
/// </summary>
internal static class CustomTypes
{
    /// <summary>Register</summary>
    internal static void Register()
    {
        PhotonPeer.RegisterType(typeof(Vector2), (byte)'W', SerializeVector2, DeserializeVector2);
        PhotonPeer.RegisterType(typeof(Vector3), (byte)'V', SerializeVector3, DeserializeVector3);
        PhotonPeer.RegisterType(typeof(Quaternion), (byte)'Q', SerializeQuaternion, DeserializeQuaternion);
        PhotonPeer.RegisterType(typeof(PhotonPlayer), (byte)'P', SerializePhotonPlayer, DeserializePhotonPlayer);
		PhotonPeer.RegisterType(typeof(GameManager), (byte)'O', SerializeGameManager, DeserializeGameManager);
		
    }

    #region Custom De/Serializer Methods

    private static byte[] SerializeVector3(object customobject)
    {
        Vector3 vo = (Vector3)customobject;
        int index = 0;

        ////TODO: check if the "float almost 0 situation is actually happening a lot in games or if this doesn't save a lot
        //byte skipFlags = 0;
        //byte floatsNeeded = 3;
        //if (vo.x < PhotonNetwork.precisionForFloatSynchronization && vo.x > -PhotonNetwork.precisionForFloatSynchronization)
        //{
        //    skipFlags += 1;
        //    floatsNeeded--;
        //}

        //if (vo.y < PhotonNetwork.precisionForFloatSynchronization && vo.y > -PhotonNetwork.precisionForFloatSynchronization)
        //{
        //    skipFlags += 2;
        //    floatsNeeded--;
        //}

        //if (vo.z < PhotonNetwork.precisionForFloatSynchronization && vo.z > -PhotonNetwork.precisionForFloatSynchronization)
        //{
        //    skipFlags += 4;
        //    floatsNeeded--;
        //}

        //if (skipFlags != 0)
        //{
        //    byte[] cutBytes = new byte[1 + floatsNeeded * 4];
        //    cutBytes[index++] = skipFlags;

        //    if ((skipFlags & 1) == 0) Protocol.Serialize(vo.x, cutBytes, ref index);
        //    if ((skipFlags & 2) == 0) Protocol.Serialize(vo.y, cutBytes, ref index);
        //    if ((skipFlags & 4) == 0) Protocol.Serialize(vo.z, cutBytes, ref index);
        //    return cutBytes;
        //}

        byte[] bytes = new byte[3 * 4];
        Protocol.Serialize(vo.x, bytes, ref index);
        Protocol.Serialize(vo.y, bytes, ref index);
        Protocol.Serialize(vo.z, bytes, ref index);
        return bytes;
    }

    private static object DeserializeVector3(byte[] bytes)
    {
        Vector3 vo = new Vector3();
        int index = 0;

        //if (bytes.Length < 12)
        //{
        //    byte skipFlags = bytes[index++];
        //    if ((skipFlags & 1) == 0) Protocol.Deserialize(out vo.x, bytes, ref index);
        //    if ((skipFlags & 2) == 0) Protocol.Deserialize(out vo.y, bytes, ref index);
        //    if ((skipFlags & 4) == 0) Protocol.Deserialize(out vo.z, bytes, ref index);

        //    return vo;
        //}

        Protocol.Deserialize(out vo.x, bytes, ref index);
        Protocol.Deserialize(out vo.y, bytes, ref index);
        Protocol.Deserialize(out vo.z, bytes, ref index);

        return vo;
    }

    private static byte[] SerializeVector2(object customobject)
    {
        Vector2 vo = (Vector2)customobject;

        byte[] bytes = new byte[2 * 4];
        int index = 0;
        Protocol.Serialize(vo.x, bytes, ref index);
        Protocol.Serialize(vo.y, bytes, ref index);
        return bytes;
    }

    private static object DeserializeVector2(byte[] bytes)
    {
        Vector2 vo = new Vector2();
        int index = 0;
        Protocol.Deserialize(out vo.x, bytes, ref index);
        Protocol.Deserialize(out vo.y, bytes, ref index);
        return vo;
    }

    private static byte[] SerializeQuaternion(object obj)
    {
        Quaternion o = (Quaternion)obj;
        
        byte[] bytes = new byte[4 * 4];
        int index = 0;
        Protocol.Serialize(o.w, bytes, ref index);
        Protocol.Serialize(o.x, bytes, ref index);
        Protocol.Serialize(o.y, bytes, ref index);
        Protocol.Serialize(o.z, bytes, ref index);
        return bytes;
    }

    private static object DeserializeQuaternion(byte[] bytes)
    {
        Quaternion o = new Quaternion();
        int index = 0;
        Protocol.Deserialize(out o.w, bytes, ref index);
        Protocol.Deserialize(out o.x, bytes, ref index);
        Protocol.Deserialize(out o.y, bytes, ref index);
        Protocol.Deserialize(out o.z, bytes, ref index);

        return o;
    }

    private static byte[] SerializePhotonPlayer(object customobject)
    {
        int ID = ((PhotonPlayer)customobject).ID;
        
        byte[] bytes = new byte[4];
        int off = 0;
        Protocol.Serialize(ID, bytes, ref off);

        return bytes;
    }

    private static object DeserializePhotonPlayer(byte[] bytes)
    {
        int ID;
        int off = 0;
        Protocol.Deserialize(out ID, bytes, ref off);

        if (PhotonNetwork.networkingPeer.mActors.ContainsKey(ID))
        {
            return PhotonNetwork.networkingPeer.mActors[ID];
        }
        else
        {
            return null;
        }
    }
	private static byte[] SerializeGameManager(object customObject)
	{
		GameManager gm = (GameManager)customObject;
		int index = 0;
		byte[] playerBytes = ExitGames.Client.Photon.Protocol.Serialize (gm.sPlayers);
		byte[] targetBytes = ExitGames.Client.Photon.Protocol.Serialize (gm.sTargets);
		byte[] bytes = new byte[playerBytes.Length + targetBytes.Length + 16];
		ExitGames.Client.Photon.Protocol.Serialize (playerBytes.Length, bytes, ref index);
		System.Array.Copy (playerBytes, 0, bytes, index, playerBytes.Length);
		index += playerBytes.Length;
		ExitGames.Client.Photon.Protocol.Serialize (targetBytes.Length, bytes, ref index);
		System.Array.Copy (targetBytes, 0, bytes, index, targetBytes.Length);
		index += targetBytes.Length;
		ExitGames.Client.Photon.Protocol.Serialize (gm.sPlayersInRoom, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Serialize (gm.sPlayersTurn, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Serialize (gm.sTargetsAlive, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Serialize (gm.sInstaniated, bytes, ref index);
		return bytes;
	}
	private static object DeserializeGameManager(byte[] bytes)
	{
		GameManager gm = new GameManager();
		int index = 0;
		int playerBytesLength;
		int targetBytesLength;
		ExitGames.Client.Photon.Protocol.Deserialize (out playerBytesLength, bytes, ref index);
		byte[] playerBytes = new byte[playerBytesLength];
		System.Array.Copy(bytes, index, playerBytes, 0, playerBytesLength);
		gm.sPlayers = ((Player[])ExitGames.Client.Photon.Protocol.Deserialize (playerBytes));
		index += playerBytesLength;
		ExitGames.Client.Photon.Protocol.Deserialize (out targetBytesLength, bytes, ref index);
		byte[] targetBytes = new byte[targetBytesLength];
		System.Array.Copy(bytes, index, targetBytes, 0, targetBytesLength);
		gm.sPlayers = ((Player[])ExitGames.Client.Photon.Protocol.Deserialize(targetBytes));
		index += targetBytesLength;
		ExitGames.Client.Photon.Protocol.Deserialize (out gm.sPlayersInRoom, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Deserialize (out gm.sPlayersTurn, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Deserialize (out gm.sTargetsAlive, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Deserialize (out gm.sInstaniated, bytes, ref index);
		return gm;
	}

    #endregion
}
