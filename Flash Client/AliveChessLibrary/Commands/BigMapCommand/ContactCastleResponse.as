// Generated by the protocol buffer compiler.  DO NOT EDIT!

package AliveChessLibrary.Commands.BigMapCommand {

  import com.google.protobuf.*;
  import flash.utils.*;
  import com.hurlant.math.BigInteger;
  import AliveChessLibrary.Commands.BigMapCommand.Dialog;
  import AliveChessLibrary.GameObjects.Buildings.Castle;
  
  public final class ContactCastleResponse extends Message {
    public function ContactCastleResponse() {
      registerField("Dispute","AliveChessLibrary.Commands.BigMapCommand.Dialog",Descriptor.MESSAGE,Descriptor.LABEL_OPTIONAL,1);
      registerField("Castle","AliveChessLibrary.GameObjects.Buildings.Castle",Descriptor.MESSAGE,Descriptor.LABEL_OPTIONAL,2);
    }
    // optional .AliveChessLibrary.Commands.BigMapCommand.Dialog _dispute = 1;
    public var Dispute:AliveChessLibrary.Commands.BigMapCommand.Dialog = null;
    
    // optional .AliveChessLibrary.Commands.BigMapCommand.Castle _castle = 2;
    public var Castle:AliveChessLibrary.GameObjects.Buildings.Castle = null;
    
  
  }
}