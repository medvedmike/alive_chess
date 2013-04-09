/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package commands.Dialog;

import commands.Util.Commands;
import commands.Util.ICommand;
import Logic.Contexts.CommandListeners.IDialogCommandListener;

/**
 *
 * @author Admin
 */
public class JoinEmperiesDialogMessage extends Message implements ICommand{
    public JoinEmperiesDialogMessage(){
        super();
        com_id = Commands.JOIN_EMPERIES_DIALOG_MESSAGE;
    }

    public void Execute(Object listener) {
        IDialogCommandListener l = (IDialogCommandListener)listener;
        l.JoinEmperiesDialogMessReceived(this);
    }


}
