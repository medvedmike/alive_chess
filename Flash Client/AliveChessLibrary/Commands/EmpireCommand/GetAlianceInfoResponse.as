// Generated by the protocol buffer compiler.  DO NOT EDIT!

package AliveChessLibrary.Commands.EmpireCommand {

  import com.google.protobuf.*;
  import flash.utils.*;
  import com.hurlant.math.BigInteger;
  import MemberInfo;
  public final class GetAlianceInfoResponse extends Message {
    public function GetAlianceInfoResponse() {
      registerField("UnionId","",Descriptor.INT32,Descriptor.LABEL_OPTIONAL,1);
      registerField("Members","MemberInfo",Descriptor.MESSAGE,Descriptor.LABEL_REPEATED,2);
    }
    // optional int32 _unionId = 1;
    public var UnionId:int = 0;
    
    // repeated .AliveChessLibrary.Commands.EmpireCommand.MemberInfo _members = 2;
    public var Members:Array = new Array();
    
    //fix bug 1 protobuf-actionscript3
    //dummy var using AliveChessLibrary.Commands.EmpireCommand. necessary to avoid following exception
    //ReferenceError: Error #1065: Variable NetworkInfo is not defined.
    //at global/flash.utils::getDefinitionByName()
    //at com.google.protobuf::Message/readFromCodedStream()
    private var MembersDummy:AliveChessLibrary.Commands.EmpireCommand.MemberInfo = null;
    
  
  }
}