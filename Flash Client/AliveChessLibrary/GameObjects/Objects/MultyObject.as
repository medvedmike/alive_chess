// Generated by the protocol buffer compiler.  DO NOT EDIT!

package AliveChessLibrary.GameObjects.Objects {

  import com.google.protobuf.*;
  import flash.utils.*;
  import com.hurlant.math.BigInteger;
  
  public final class MultyObject extends Message {
    public function MultyObject() {
      registerField("MultyObjectId","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,1);
      registerField("LeftX","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,2);
      registerField("TopY","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,3);
      registerField("Width","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,4);
      registerField("Height","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,5);
      registerField("WayCost","",Descriptor.FIXED32,Descriptor.LABEL_OPTIONAL,6);
      registerField("MultyObjectType","AliveChessLibrary.GameObjects.Objects.MultyObjectTypes",Descriptor.ENUM,Descriptor.LABEL_OPTIONAL,7);
    }
    // optional int32 _multyObjectId = 1;
    public var MultyObjectId:int = 0;
    
    // optional int32 _leftX = 2;
    public var LeftX:int = 0;
    
    // optional int32 _topY = 3;
    public var TopY:int = 0;
    
    // optional int32 _width = 4;
    public var Width:int = 0;
    
    // optional int32 _height = 5;
    public var Height:int = 0;
    
    // optional fixed32 _wayCost = 6;
    public var WayCost:int = 0;
    
    // optional .AliveChessLibrary.GameObjects.Objects.MultyObjectTypes _multyObjectType = 7;
    public var MultyObjectType:Number = -1; //No default value for now...
    
  
  }
}