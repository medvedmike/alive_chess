/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package commands.Castle;

import commands.Util.Commands;
import Serializer.Utils.FieldSerializer;
import Serializer.Utils.IProtoSerializableRequest;
import java.io.IOException;

/**
 *
 * @author Admin
 */
public class ShowArmyCastleRequest implements IProtoSerializableRequest{
    private int com_id;

    public ShowArmyCastleRequest(){
        com_id = Commands.SHOW_ARMY_CASTLE_REQUEST;
    }

    public int ComputeSize() {
        return 0;
    }

    public byte[] toByte() throws IOException{
        int len = ComputeSize();
        byte[] result = new byte[len+8];
        FieldSerializer sr = new FieldSerializer(result);
        sr.WriteIntNonSerialized(com_id);
        sr.WriteIntNonSerialized(len);
        return result;
    }
}
