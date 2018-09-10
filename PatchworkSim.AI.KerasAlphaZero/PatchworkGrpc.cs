// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: patchwork.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

public static partial class PatchworkServer
{
  static readonly string __ServiceName = "PatchworkServer";

  static readonly grpc::Marshaller<global::StaticConfigRequest> __Marshaller_StaticConfigRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::StaticConfigRequest.Parser.ParseFrom);
  static readonly grpc::Marshaller<global::StaticConfigReply> __Marshaller_StaticConfigReply = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::StaticConfigReply.Parser.ParseFrom);
  static readonly grpc::Marshaller<global::EvaluateRequest> __Marshaller_EvaluateRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EvaluateRequest.Parser.ParseFrom);
  static readonly grpc::Marshaller<global::EvaluateReply> __Marshaller_EvaluateReply = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EvaluateReply.Parser.ParseFrom);
  static readonly grpc::Marshaller<global::TrainRequest> __Marshaller_TrainRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::TrainRequest.Parser.ParseFrom);
  static readonly grpc::Marshaller<global::TrainReply> __Marshaller_TrainReply = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::TrainReply.Parser.ParseFrom);

  static readonly grpc::Method<global::StaticConfigRequest, global::StaticConfigReply> __Method_GetStaticConfig = new grpc::Method<global::StaticConfigRequest, global::StaticConfigReply>(
      grpc::MethodType.Unary,
      __ServiceName,
      "GetStaticConfig",
      __Marshaller_StaticConfigRequest,
      __Marshaller_StaticConfigReply);

  static readonly grpc::Method<global::EvaluateRequest, global::EvaluateReply> __Method_Evaluate = new grpc::Method<global::EvaluateRequest, global::EvaluateReply>(
      grpc::MethodType.Unary,
      __ServiceName,
      "Evaluate",
      __Marshaller_EvaluateRequest,
      __Marshaller_EvaluateReply);

  static readonly grpc::Method<global::TrainRequest, global::TrainReply> __Method_Train = new grpc::Method<global::TrainRequest, global::TrainReply>(
      grpc::MethodType.Unary,
      __ServiceName,
      "Train",
      __Marshaller_TrainRequest,
      __Marshaller_TrainReply);

  /// <summary>Service descriptor</summary>
  public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
  {
    get { return global::PatchworkReflection.Descriptor.Services[0]; }
  }

  /// <summary>Base class for server-side implementations of PatchworkServer</summary>
  public abstract partial class PatchworkServerBase
  {
    public virtual global::System.Threading.Tasks.Task<global::StaticConfigReply> GetStaticConfig(global::StaticConfigRequest request, grpc::ServerCallContext context)
    {
      throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
    }

    public virtual global::System.Threading.Tasks.Task<global::EvaluateReply> Evaluate(global::EvaluateRequest request, grpc::ServerCallContext context)
    {
      throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
    }

    public virtual global::System.Threading.Tasks.Task<global::TrainReply> Train(global::TrainRequest request, grpc::ServerCallContext context)
    {
      throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
    }

  }

  /// <summary>Client for PatchworkServer</summary>
  public partial class PatchworkServerClient : grpc::ClientBase<PatchworkServerClient>
  {
    /// <summary>Creates a new client for PatchworkServer</summary>
    /// <param name="channel">The channel to use to make remote calls.</param>
    public PatchworkServerClient(grpc::Channel channel) : base(channel)
    {
    }
    /// <summary>Creates a new client for PatchworkServer that uses a custom <c>CallInvoker</c>.</summary>
    /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
    public PatchworkServerClient(grpc::CallInvoker callInvoker) : base(callInvoker)
    {
    }
    /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
    protected PatchworkServerClient() : base()
    {
    }
    /// <summary>Protected constructor to allow creation of configured clients.</summary>
    /// <param name="configuration">The client configuration.</param>
    protected PatchworkServerClient(ClientBaseConfiguration configuration) : base(configuration)
    {
    }

    public virtual global::StaticConfigReply GetStaticConfig(global::StaticConfigRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
    {
      return GetStaticConfig(request, new grpc::CallOptions(headers, deadline, cancellationToken));
    }
    public virtual global::StaticConfigReply GetStaticConfig(global::StaticConfigRequest request, grpc::CallOptions options)
    {
      return CallInvoker.BlockingUnaryCall(__Method_GetStaticConfig, null, options, request);
    }
    public virtual grpc::AsyncUnaryCall<global::StaticConfigReply> GetStaticConfigAsync(global::StaticConfigRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
    {
      return GetStaticConfigAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
    }
    public virtual grpc::AsyncUnaryCall<global::StaticConfigReply> GetStaticConfigAsync(global::StaticConfigRequest request, grpc::CallOptions options)
    {
      return CallInvoker.AsyncUnaryCall(__Method_GetStaticConfig, null, options, request);
    }
    public virtual global::EvaluateReply Evaluate(global::EvaluateRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
    {
      return Evaluate(request, new grpc::CallOptions(headers, deadline, cancellationToken));
    }
    public virtual global::EvaluateReply Evaluate(global::EvaluateRequest request, grpc::CallOptions options)
    {
      return CallInvoker.BlockingUnaryCall(__Method_Evaluate, null, options, request);
    }
    public virtual grpc::AsyncUnaryCall<global::EvaluateReply> EvaluateAsync(global::EvaluateRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
    {
      return EvaluateAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
    }
    public virtual grpc::AsyncUnaryCall<global::EvaluateReply> EvaluateAsync(global::EvaluateRequest request, grpc::CallOptions options)
    {
      return CallInvoker.AsyncUnaryCall(__Method_Evaluate, null, options, request);
    }
    public virtual global::TrainReply Train(global::TrainRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
    {
      return Train(request, new grpc::CallOptions(headers, deadline, cancellationToken));
    }
    public virtual global::TrainReply Train(global::TrainRequest request, grpc::CallOptions options)
    {
      return CallInvoker.BlockingUnaryCall(__Method_Train, null, options, request);
    }
    public virtual grpc::AsyncUnaryCall<global::TrainReply> TrainAsync(global::TrainRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
    {
      return TrainAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
    }
    public virtual grpc::AsyncUnaryCall<global::TrainReply> TrainAsync(global::TrainRequest request, grpc::CallOptions options)
    {
      return CallInvoker.AsyncUnaryCall(__Method_Train, null, options, request);
    }
    /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
    protected override PatchworkServerClient NewInstance(ClientBaseConfiguration configuration)
    {
      return new PatchworkServerClient(configuration);
    }
  }

  /// <summary>Creates service definition that can be registered with a server</summary>
  /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
  public static grpc::ServerServiceDefinition BindService(PatchworkServerBase serviceImpl)
  {
    return grpc::ServerServiceDefinition.CreateBuilder()
        .AddMethod(__Method_GetStaticConfig, serviceImpl.GetStaticConfig)
        .AddMethod(__Method_Evaluate, serviceImpl.Evaluate)
        .AddMethod(__Method_Train, serviceImpl.Train).Build();
  }

}
#endregion
