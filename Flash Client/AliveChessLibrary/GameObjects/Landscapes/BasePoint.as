// Generated by the protocol buffer compiler.  DO NOT EDIT!

package AliveChessLibrary.GameObjects.Landscapes {

  import com.google.protobuf.*;
  import flash.utils.*;
  import com.hurlant.math.BigInteger;
  public final class BasePoint extends Message {
    public function BasePoint() {
      registerField("BasePointId","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,1);
      registerField("X","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,2);
      registerField("Y","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,3);
      registerField("LandscapePointType","AliveChessLibrary.GameObjects.Landscapes.LandscapeTypes",Descriptor.ENUM,Descriptor.LABEL_OPTIONAL,4);
      registerField("WayCost","",Descriptor.FIXED32,Descriptor.LABEL_OPTIONAL,5);
    }
    // optional int32 _basePointId = 1;
    public var BasePointId:int = 0;
    
    // optional int32 _x = 2;
    public var X:int = 0;
    
    // optional int32 _y = 3;
    public var Y:int = 0;
    
    // optional .AliveChessLibrary.GameObjects.Landscapes.LandscapeTypes _landscapePointType = 4;
    public var LandscapePointType:Number = -1; //No default value for now...
    
    // optional fixed32 _wayCost = 5;
    public var WayCost:int = 0;
    
  
  }
}