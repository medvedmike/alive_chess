// Generated by the protocol buffer compiler.  DO NOT EDIT!

package AliveChessLibrary.Commands.BigMapCommand {

  import com.google.protobuf.*;
  import flash.utils.*;
  import com.hurlant.math.BigInteger;
  import AliveChessLibrary.Commands.BigMapCommand.Position;
  public final class MoveKingResponse extends Message {
    public function MoveKingResponse() {
      registerField("steps","AliveChessLibrary.Commands.BigMapCommand.Position",Descriptor.MESSAGE,Descriptor.LABEL_REPEATED,1);
    }
    // repeated .AliveChessLibrary.Commands.BigMapCommand.Position steps = 1;
    public var steps:Array = new Array();
    
    //fix bug 1 protobuf-actionscript3
    //dummy var using AliveChessLibrary.Commands.BigMapCommand. necessary to avoid following exception
    //ReferenceError: Error #1065: Variable NetworkInfo is not defined.
    //at global/flash.utils::getDefinitionByName()
    //at com.google.protobuf::Message/readFromCodedStream()
    private var stepsDummy:AliveChessLibrary.Commands.BigMapCommand.Position = null;
    
  
  }
}