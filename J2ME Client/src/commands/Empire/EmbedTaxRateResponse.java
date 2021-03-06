/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package commands.Empire;

import commands.Util.ICommand;
import Logic.Contexts.CommandListeners.IEmpireCommandListener;
import Serializer.Utils.FieldDeserializer;
import Serializer.Utils.IProtoDeserializable;
import java.io.IOException;

/**
 *
 * @author Admin
 */
public class EmbedTaxRateResponse implements IProtoDeserializable, ICommand {

    private boolean succeed;    //proto 1

    public EmbedTaxRateResponse(){}

    public boolean getSucceed(){return succeed;}

    public void LoadFrom(byte[] buffer) throws IOException {
        FieldDeserializer dsr = new FieldDeserializer(buffer);
        succeed = dsr.readBool(1);
    }

    public void Execute(Object listener) {
        IEmpireCommandListener l = (IEmpireCommandListener)listener;
        l.EmbedTaxRateResponseReceived(this);
    }


}
