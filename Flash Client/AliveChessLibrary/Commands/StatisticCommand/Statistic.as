// Generated by the protocol buffer compiler.  DO NOT EDIT!

package AliveChessLibrary.Commands.StatisticCommand {

  import com.google.protobuf.*;
  import flash.utils.*;
  import com.hurlant.math.BigInteger;
  public final class Statistic extends Message {
    public function Statistic() {
      registerField("PawnNumber","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,1);
      registerField("BishopNumber","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,2);
      registerField("RookNumber","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,3);
      registerField("QueenNumber","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,4);
      registerField("KnightNumber","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,5);
    }
    // optional int32 _pawnNumber = 1;
    public var PawnNumber:int = 0;
    
    // optional int32 _bishopNumber = 2;
    public var BishopNumber:int = 0;
    
    // optional int32 _rookNumber = 3;
    public var RookNumber:int = 0;
    
    // optional int32 _queenNumber = 4;
    public var QueenNumber:int = 0;
    
    // optional int32 _knightNumber = 5;
    public var KnightNumber:int = 0;
    
  
  }
}