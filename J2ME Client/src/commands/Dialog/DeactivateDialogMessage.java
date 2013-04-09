/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package commands.Dialog;

import commands.Util.Commands;
import commands.Util.ICommand;
import Logic.Contexts.CommandListeners.IDialogCommandListener;
import Serializer.Utils.FieldSerializer;
import Serializer.Utils.IProtoDeserializable;
import Serializer.Utils.IProtoSerializableRequest;
import java.io.IOException;

/**
 *
 * @author Admin
 */
public class DeactivateDialogMessage implements IProtoDeserializable, IProtoSerializableRequest, ICommand {
    private int com_id;

    public DeactivateDialogMessage(){com_id = Commands.DEACTIVATE_DIALOG_MESSAGE;}

    public int ComputeSize() {
        return 0;
    }

    public byte[] toByte() throws IOException {
        int len = ComputeSize();
        byte[] result = new byte[len+8];
        FieldSerializer sr = new FieldSerializer(result);
        sr.WriteIntNonSerialized(com_id);
        sr.WriteIntNonSerialized(len);
        return result;
    }

    public void LoadFrom(byte[] buffer) throws IOException {

    }

    public void Execute(Object listener) {
        IDialogCommandListener l = (IDialogCommandListener)listener;
        l.DeactivateDialogMessReceived(this);
    }


}
