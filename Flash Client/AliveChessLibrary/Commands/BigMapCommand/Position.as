// Generated by the protocol buffer compiler.  DO NOT EDIT!

package AliveChessLibrary.Commands.BigMapCommand {

  import com.google.protobuf.*;
  import flash.utils.*;
  import com.hurlant.math.BigInteger;
  public final class Position extends Message {
    public function Position() {
      registerField("X","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,1);
      registerField("Y","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,2);
    }
    // optional int32 _x = 1;
    public var X:int = 0;
    
    // optional int32 _y = 2;
    public var Y:int = 0;
    
  
  }
}