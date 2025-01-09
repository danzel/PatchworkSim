// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: patchwork.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from patchwork.proto</summary>
public static partial class PatchworkReflection {

  #region Descriptor
  /// <summary>File descriptor for patchwork.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static PatchworkReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "Cg9wYXRjaHdvcmsucHJvdG8iFQoTU3RhdGljQ29uZmlnUmVxdWVzdCIsChFT",
          "dGF0aWNDb25maWdSZXBseRIXCg9vYnNlcnZhdGlvblNpemUYASABKAUiKQoN",
          "Q3JlYXRlUmVxdWVzdBIYChBvcHBvbmVudFN0cmVuZ3RoGAEgASgFIkAKC0Ny",
          "ZWF0ZVJlcGx5Eg4KBmdhbWVJZBgBIAEoBRIhCgtvYnNlcnZhdGlvbhgCIAEo",
          "CzIMLk9ic2VydmF0aW9uIisKC01vdmVSZXF1ZXN0Eg4KBmdhbWVJZBgBIAEo",
          "BRIMCgRtb3ZlGAIgASgFIlsKCU1vdmVSZXBseRIUCgxnYW1lSGFzRW5kZWQY",
          "ASABKAgSFQoNd2lubmluZ1BsYXllchgCIAEoBRIhCgtvYnNlcnZhdGlvbhgD",
          "IAEoCzIMLk9ic2VydmF0aW9uIj0KC09ic2VydmF0aW9uEg4KBnJld2FyZBgB",
          "IAEoAhIeChZvYnNlcnZhdGlvbkZvck5leHRNb3ZlGAIgAygCMqUBCg9QYXRj",
          "aHdvcmtTZXJ2ZXISPQoPR2V0U3RhdGljQ29uZmlnEhQuU3RhdGljQ29uZmln",
          "UmVxdWVzdBoSLlN0YXRpY0NvbmZpZ1JlcGx5IgASKAoGQ3JlYXRlEg4uQ3Jl",
          "YXRlUmVxdWVzdBoMLkNyZWF0ZVJlcGx5IgASKQoLUGVyZm9ybU1vdmUSDC5N",
          "b3ZlUmVxdWVzdBoKLk1vdmVSZXBseSIAYgZwcm90bzM="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::StaticConfigRequest), global::StaticConfigRequest.Parser, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::StaticConfigReply), global::StaticConfigReply.Parser, new[]{ "ObservationSize" }, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::CreateRequest), global::CreateRequest.Parser, new[]{ "OpponentStrength" }, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::CreateReply), global::CreateReply.Parser, new[]{ "GameId", "Observation" }, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::MoveRequest), global::MoveRequest.Parser, new[]{ "GameId", "Move" }, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::MoveReply), global::MoveReply.Parser, new[]{ "GameHasEnded", "WinningPlayer", "Observation" }, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::Observation), global::Observation.Parser, new[]{ "Reward", "ObservationForNextMove" }, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class StaticConfigRequest : pb::IMessage<StaticConfigRequest> {
  private static readonly pb::MessageParser<StaticConfigRequest> _parser = new pb::MessageParser<StaticConfigRequest>(() => new StaticConfigRequest());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<StaticConfigRequest> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PatchworkReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public StaticConfigRequest() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public StaticConfigRequest(StaticConfigRequest other) : this() {
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public StaticConfigRequest Clone() {
    return new StaticConfigRequest(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as StaticConfigRequest);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(StaticConfigRequest other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(StaticConfigRequest other) {
    if (other == null) {
      return;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
      }
    }
  }

}

public sealed partial class StaticConfigReply : pb::IMessage<StaticConfigReply> {
  private static readonly pb::MessageParser<StaticConfigReply> _parser = new pb::MessageParser<StaticConfigReply>(() => new StaticConfigReply());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<StaticConfigReply> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PatchworkReflection.Descriptor.MessageTypes[1]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public StaticConfigReply() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public StaticConfigReply(StaticConfigReply other) : this() {
    observationSize_ = other.observationSize_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public StaticConfigReply Clone() {
    return new StaticConfigReply(this);
  }

  /// <summary>Field number for the "observationSize" field.</summary>
  public const int ObservationSizeFieldNumber = 1;
  private int observationSize_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int ObservationSize {
    get { return observationSize_; }
    set {
      observationSize_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as StaticConfigReply);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(StaticConfigReply other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (ObservationSize != other.ObservationSize) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (ObservationSize != 0) hash ^= ObservationSize.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (ObservationSize != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(ObservationSize);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (ObservationSize != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(ObservationSize);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(StaticConfigReply other) {
    if (other == null) {
      return;
    }
    if (other.ObservationSize != 0) {
      ObservationSize = other.ObservationSize;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          ObservationSize = input.ReadInt32();
          break;
        }
      }
    }
  }

}

public sealed partial class CreateRequest : pb::IMessage<CreateRequest> {
  private static readonly pb::MessageParser<CreateRequest> _parser = new pb::MessageParser<CreateRequest>(() => new CreateRequest());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<CreateRequest> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PatchworkReflection.Descriptor.MessageTypes[2]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public CreateRequest() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public CreateRequest(CreateRequest other) : this() {
    opponentStrength_ = other.opponentStrength_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public CreateRequest Clone() {
    return new CreateRequest(this);
  }

  /// <summary>Field number for the "opponentStrength" field.</summary>
  public const int OpponentStrengthFieldNumber = 1;
  private int opponentStrength_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int OpponentStrength {
    get { return opponentStrength_; }
    set {
      opponentStrength_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as CreateRequest);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(CreateRequest other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (OpponentStrength != other.OpponentStrength) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (OpponentStrength != 0) hash ^= OpponentStrength.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (OpponentStrength != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(OpponentStrength);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (OpponentStrength != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(OpponentStrength);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(CreateRequest other) {
    if (other == null) {
      return;
    }
    if (other.OpponentStrength != 0) {
      OpponentStrength = other.OpponentStrength;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          OpponentStrength = input.ReadInt32();
          break;
        }
      }
    }
  }

}

public sealed partial class CreateReply : pb::IMessage<CreateReply> {
  private static readonly pb::MessageParser<CreateReply> _parser = new pb::MessageParser<CreateReply>(() => new CreateReply());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<CreateReply> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PatchworkReflection.Descriptor.MessageTypes[3]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public CreateReply() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public CreateReply(CreateReply other) : this() {
    gameId_ = other.gameId_;
    Observation = other.observation_ != null ? other.Observation.Clone() : null;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public CreateReply Clone() {
    return new CreateReply(this);
  }

  /// <summary>Field number for the "gameId" field.</summary>
  public const int GameIdFieldNumber = 1;
  private int gameId_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int GameId {
    get { return gameId_; }
    set {
      gameId_ = value;
    }
  }

  /// <summary>Field number for the "observation" field.</summary>
  public const int ObservationFieldNumber = 2;
  private global::Observation observation_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::Observation Observation {
    get { return observation_; }
    set {
      observation_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as CreateReply);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(CreateReply other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (GameId != other.GameId) return false;
    if (!object.Equals(Observation, other.Observation)) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (GameId != 0) hash ^= GameId.GetHashCode();
    if (observation_ != null) hash ^= Observation.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (GameId != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(GameId);
    }
    if (observation_ != null) {
      output.WriteRawTag(18);
      output.WriteMessage(Observation);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (GameId != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(GameId);
    }
    if (observation_ != null) {
      size += 1 + pb::CodedOutputStream.ComputeMessageSize(Observation);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(CreateReply other) {
    if (other == null) {
      return;
    }
    if (other.GameId != 0) {
      GameId = other.GameId;
    }
    if (other.observation_ != null) {
      if (observation_ == null) {
        observation_ = new global::Observation();
      }
      Observation.MergeFrom(other.Observation);
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          GameId = input.ReadInt32();
          break;
        }
        case 18: {
          if (observation_ == null) {
            observation_ = new global::Observation();
          }
          input.ReadMessage(observation_);
          break;
        }
      }
    }
  }

}

public sealed partial class MoveRequest : pb::IMessage<MoveRequest> {
  private static readonly pb::MessageParser<MoveRequest> _parser = new pb::MessageParser<MoveRequest>(() => new MoveRequest());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<MoveRequest> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PatchworkReflection.Descriptor.MessageTypes[4]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public MoveRequest() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public MoveRequest(MoveRequest other) : this() {
    gameId_ = other.gameId_;
    move_ = other.move_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public MoveRequest Clone() {
    return new MoveRequest(this);
  }

  /// <summary>Field number for the "gameId" field.</summary>
  public const int GameIdFieldNumber = 1;
  private int gameId_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int GameId {
    get { return gameId_; }
    set {
      gameId_ = value;
    }
  }

  /// <summary>Field number for the "move" field.</summary>
  public const int MoveFieldNumber = 2;
  private int move_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int Move {
    get { return move_; }
    set {
      move_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as MoveRequest);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(MoveRequest other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (GameId != other.GameId) return false;
    if (Move != other.Move) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (GameId != 0) hash ^= GameId.GetHashCode();
    if (Move != 0) hash ^= Move.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (GameId != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(GameId);
    }
    if (Move != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(Move);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (GameId != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(GameId);
    }
    if (Move != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(Move);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(MoveRequest other) {
    if (other == null) {
      return;
    }
    if (other.GameId != 0) {
      GameId = other.GameId;
    }
    if (other.Move != 0) {
      Move = other.Move;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          GameId = input.ReadInt32();
          break;
        }
        case 16: {
          Move = input.ReadInt32();
          break;
        }
      }
    }
  }

}

public sealed partial class MoveReply : pb::IMessage<MoveReply> {
  private static readonly pb::MessageParser<MoveReply> _parser = new pb::MessageParser<MoveReply>(() => new MoveReply());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<MoveReply> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PatchworkReflection.Descriptor.MessageTypes[5]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public MoveReply() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public MoveReply(MoveReply other) : this() {
    gameHasEnded_ = other.gameHasEnded_;
    winningPlayer_ = other.winningPlayer_;
    Observation = other.observation_ != null ? other.Observation.Clone() : null;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public MoveReply Clone() {
    return new MoveReply(this);
  }

  /// <summary>Field number for the "gameHasEnded" field.</summary>
  public const int GameHasEndedFieldNumber = 1;
  private bool gameHasEnded_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool GameHasEnded {
    get { return gameHasEnded_; }
    set {
      gameHasEnded_ = value;
    }
  }

  /// <summary>Field number for the "winningPlayer" field.</summary>
  public const int WinningPlayerFieldNumber = 2;
  private int winningPlayer_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int WinningPlayer {
    get { return winningPlayer_; }
    set {
      winningPlayer_ = value;
    }
  }

  /// <summary>Field number for the "observation" field.</summary>
  public const int ObservationFieldNumber = 3;
  private global::Observation observation_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::Observation Observation {
    get { return observation_; }
    set {
      observation_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as MoveReply);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(MoveReply other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (GameHasEnded != other.GameHasEnded) return false;
    if (WinningPlayer != other.WinningPlayer) return false;
    if (!object.Equals(Observation, other.Observation)) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (GameHasEnded != false) hash ^= GameHasEnded.GetHashCode();
    if (WinningPlayer != 0) hash ^= WinningPlayer.GetHashCode();
    if (observation_ != null) hash ^= Observation.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (GameHasEnded != false) {
      output.WriteRawTag(8);
      output.WriteBool(GameHasEnded);
    }
    if (WinningPlayer != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(WinningPlayer);
    }
    if (observation_ != null) {
      output.WriteRawTag(26);
      output.WriteMessage(Observation);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (GameHasEnded != false) {
      size += 1 + 1;
    }
    if (WinningPlayer != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(WinningPlayer);
    }
    if (observation_ != null) {
      size += 1 + pb::CodedOutputStream.ComputeMessageSize(Observation);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(MoveReply other) {
    if (other == null) {
      return;
    }
    if (other.GameHasEnded != false) {
      GameHasEnded = other.GameHasEnded;
    }
    if (other.WinningPlayer != 0) {
      WinningPlayer = other.WinningPlayer;
    }
    if (other.observation_ != null) {
      if (observation_ == null) {
        observation_ = new global::Observation();
      }
      Observation.MergeFrom(other.Observation);
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          GameHasEnded = input.ReadBool();
          break;
        }
        case 16: {
          WinningPlayer = input.ReadInt32();
          break;
        }
        case 26: {
          if (observation_ == null) {
            observation_ = new global::Observation();
          }
          input.ReadMessage(observation_);
          break;
        }
      }
    }
  }

}

public sealed partial class Observation : pb::IMessage<Observation> {
  private static readonly pb::MessageParser<Observation> _parser = new pb::MessageParser<Observation>(() => new Observation());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<Observation> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PatchworkReflection.Descriptor.MessageTypes[6]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Observation() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Observation(Observation other) : this() {
    reward_ = other.reward_;
    observationForNextMove_ = other.observationForNextMove_.Clone();
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Observation Clone() {
    return new Observation(this);
  }

  /// <summary>Field number for the "reward" field.</summary>
  public const int RewardFieldNumber = 1;
  private float reward_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public float Reward {
    get { return reward_; }
    set {
      reward_ = value;
    }
  }

  /// <summary>Field number for the "observationForNextMove" field.</summary>
  public const int ObservationForNextMoveFieldNumber = 2;
  private static readonly pb::FieldCodec<float> _repeated_observationForNextMove_codec
      = pb::FieldCodec.ForFloat(18);
  private readonly pbc::RepeatedField<float> observationForNextMove_ = new pbc::RepeatedField<float>();
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public pbc::RepeatedField<float> ObservationForNextMove {
    get { return observationForNextMove_; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as Observation);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(Observation other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Reward, other.Reward)) return false;
    if(!observationForNextMove_.Equals(other.observationForNextMove_)) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Reward != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Reward);
    hash ^= observationForNextMove_.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (Reward != 0F) {
      output.WriteRawTag(13);
      output.WriteFloat(Reward);
    }
    observationForNextMove_.WriteTo(output, _repeated_observationForNextMove_codec);
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Reward != 0F) {
      size += 1 + 4;
    }
    size += observationForNextMove_.CalculateSize(_repeated_observationForNextMove_codec);
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(Observation other) {
    if (other == null) {
      return;
    }
    if (other.Reward != 0F) {
      Reward = other.Reward;
    }
    observationForNextMove_.Add(other.observationForNextMove_);
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 13: {
          Reward = input.ReadFloat();
          break;
        }
        case 18:
        case 21: {
          observationForNextMove_.AddEntriesFrom(input, _repeated_observationForNextMove_codec);
          break;
        }
      }
    }
  }

}

#endregion


#endregion Designer generated code
