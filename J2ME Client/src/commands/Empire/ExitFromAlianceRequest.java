/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package commands.Empire;

import commands.Util.Commands;
import Serializer.Utils.FieldSerializer;
import Serializer.Utils.IProtoSerializableRequest;
import java.io.IOException;

/**
 *
 * @author Admin
 */
public class ExitFromAlianceRequest implements IProtoSerializableRequest {

    private int com_id;

    public ExitFromAlianceRequest(){
        com_id = Commands.EXIT_FROM_ALIANCE_REQUEST;
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
