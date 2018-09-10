# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: patchwork.proto

import sys
_b=sys.version_info[0]<3 and (lambda x:x) or (lambda x:x.encode('latin1'))
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
from google.protobuf import descriptor_pb2
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor.FileDescriptor(
  name='patchwork.proto',
  package='',
  syntax='proto3',
  serialized_pb=_b('\n\x0fpatchwork.proto\"\x15\n\x13StaticConfigRequest\",\n\x11StaticConfigReply\x12\x17\n\x0fobservationSize\x18\x01 \x01(\x05\" \n\tGameState\x12\x13\n\x0bobservation\x18\x01 \x03(\x02\",\n\x0f\x45valuateRequest\x12\x19\n\x05state\x18\x01 \x03(\x0b\x32\n.GameState\"1\n\nEvaluation\x12\x0f\n\x07winRate\x18\x01 \x01(\x02\x12\x12\n\nmoveRating\x18\x02 \x03(\x02\"1\n\rEvaluateReply\x12 \n\x0b\x65valuations\x18\x01 \x03(\x0b\x32\x0b.Evaluation\"K\n\x0bTrainSample\x12\x19\n\x05state\x18\x01 \x01(\x0b\x32\n.GameState\x12\r\n\x05isWin\x18\x02 \x01(\x08\x12\x12\n\nmoveRating\x18\x03 \x03(\x02\"-\n\x0cTrainRequest\x12\x1d\n\x07samples\x18\x01 \x03(\x0b\x32\x0c.TrainSample\"\x0c\n\nTrainReply2\xa7\x01\n\x0fPatchworkServer\x12=\n\x0fGetStaticConfig\x12\x14.StaticConfigRequest\x1a\x12.StaticConfigReply\"\x00\x12.\n\x08\x45valuate\x12\x10.EvaluateRequest\x1a\x0e.EvaluateReply\"\x00\x12%\n\x05Train\x12\r.TrainRequest\x1a\x0b.TrainReply\"\x00\x62\x06proto3')
)




_STATICCONFIGREQUEST = _descriptor.Descriptor(
  name='StaticConfigRequest',
  full_name='StaticConfigRequest',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=19,
  serialized_end=40,
)


_STATICCONFIGREPLY = _descriptor.Descriptor(
  name='StaticConfigReply',
  full_name='StaticConfigReply',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='observationSize', full_name='StaticConfigReply.observationSize', index=0,
      number=1, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=42,
  serialized_end=86,
)


_GAMESTATE = _descriptor.Descriptor(
  name='GameState',
  full_name='GameState',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='observation', full_name='GameState.observation', index=0,
      number=1, type=2, cpp_type=6, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=88,
  serialized_end=120,
)


_EVALUATEREQUEST = _descriptor.Descriptor(
  name='EvaluateRequest',
  full_name='EvaluateRequest',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='state', full_name='EvaluateRequest.state', index=0,
      number=1, type=11, cpp_type=10, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=122,
  serialized_end=166,
)


_EVALUATION = _descriptor.Descriptor(
  name='Evaluation',
  full_name='Evaluation',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='winRate', full_name='Evaluation.winRate', index=0,
      number=1, type=2, cpp_type=6, label=1,
      has_default_value=False, default_value=float(0),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='moveRating', full_name='Evaluation.moveRating', index=1,
      number=2, type=2, cpp_type=6, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=168,
  serialized_end=217,
)


_EVALUATEREPLY = _descriptor.Descriptor(
  name='EvaluateReply',
  full_name='EvaluateReply',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='evaluations', full_name='EvaluateReply.evaluations', index=0,
      number=1, type=11, cpp_type=10, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=219,
  serialized_end=268,
)


_TRAINSAMPLE = _descriptor.Descriptor(
  name='TrainSample',
  full_name='TrainSample',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='state', full_name='TrainSample.state', index=0,
      number=1, type=11, cpp_type=10, label=1,
      has_default_value=False, default_value=None,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='isWin', full_name='TrainSample.isWin', index=1,
      number=2, type=8, cpp_type=7, label=1,
      has_default_value=False, default_value=False,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='moveRating', full_name='TrainSample.moveRating', index=2,
      number=3, type=2, cpp_type=6, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=270,
  serialized_end=345,
)


_TRAINREQUEST = _descriptor.Descriptor(
  name='TrainRequest',
  full_name='TrainRequest',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='samples', full_name='TrainRequest.samples', index=0,
      number=1, type=11, cpp_type=10, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=347,
  serialized_end=392,
)


_TRAINREPLY = _descriptor.Descriptor(
  name='TrainReply',
  full_name='TrainReply',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=394,
  serialized_end=406,
)

_EVALUATEREQUEST.fields_by_name['state'].message_type = _GAMESTATE
_EVALUATEREPLY.fields_by_name['evaluations'].message_type = _EVALUATION
_TRAINSAMPLE.fields_by_name['state'].message_type = _GAMESTATE
_TRAINREQUEST.fields_by_name['samples'].message_type = _TRAINSAMPLE
DESCRIPTOR.message_types_by_name['StaticConfigRequest'] = _STATICCONFIGREQUEST
DESCRIPTOR.message_types_by_name['StaticConfigReply'] = _STATICCONFIGREPLY
DESCRIPTOR.message_types_by_name['GameState'] = _GAMESTATE
DESCRIPTOR.message_types_by_name['EvaluateRequest'] = _EVALUATEREQUEST
DESCRIPTOR.message_types_by_name['Evaluation'] = _EVALUATION
DESCRIPTOR.message_types_by_name['EvaluateReply'] = _EVALUATEREPLY
DESCRIPTOR.message_types_by_name['TrainSample'] = _TRAINSAMPLE
DESCRIPTOR.message_types_by_name['TrainRequest'] = _TRAINREQUEST
DESCRIPTOR.message_types_by_name['TrainReply'] = _TRAINREPLY
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

StaticConfigRequest = _reflection.GeneratedProtocolMessageType('StaticConfigRequest', (_message.Message,), dict(
  DESCRIPTOR = _STATICCONFIGREQUEST,
  __module__ = 'patchwork_pb2'
  # @@protoc_insertion_point(class_scope:StaticConfigRequest)
  ))
_sym_db.RegisterMessage(StaticConfigRequest)

StaticConfigReply = _reflection.GeneratedProtocolMessageType('StaticConfigReply', (_message.Message,), dict(
  DESCRIPTOR = _STATICCONFIGREPLY,
  __module__ = 'patchwork_pb2'
  # @@protoc_insertion_point(class_scope:StaticConfigReply)
  ))
_sym_db.RegisterMessage(StaticConfigReply)

GameState = _reflection.GeneratedProtocolMessageType('GameState', (_message.Message,), dict(
  DESCRIPTOR = _GAMESTATE,
  __module__ = 'patchwork_pb2'
  # @@protoc_insertion_point(class_scope:GameState)
  ))
_sym_db.RegisterMessage(GameState)

EvaluateRequest = _reflection.GeneratedProtocolMessageType('EvaluateRequest', (_message.Message,), dict(
  DESCRIPTOR = _EVALUATEREQUEST,
  __module__ = 'patchwork_pb2'
  # @@protoc_insertion_point(class_scope:EvaluateRequest)
  ))
_sym_db.RegisterMessage(EvaluateRequest)

Evaluation = _reflection.GeneratedProtocolMessageType('Evaluation', (_message.Message,), dict(
  DESCRIPTOR = _EVALUATION,
  __module__ = 'patchwork_pb2'
  # @@protoc_insertion_point(class_scope:Evaluation)
  ))
_sym_db.RegisterMessage(Evaluation)

EvaluateReply = _reflection.GeneratedProtocolMessageType('EvaluateReply', (_message.Message,), dict(
  DESCRIPTOR = _EVALUATEREPLY,
  __module__ = 'patchwork_pb2'
  # @@protoc_insertion_point(class_scope:EvaluateReply)
  ))
_sym_db.RegisterMessage(EvaluateReply)

TrainSample = _reflection.GeneratedProtocolMessageType('TrainSample', (_message.Message,), dict(
  DESCRIPTOR = _TRAINSAMPLE,
  __module__ = 'patchwork_pb2'
  # @@protoc_insertion_point(class_scope:TrainSample)
  ))
_sym_db.RegisterMessage(TrainSample)

TrainRequest = _reflection.GeneratedProtocolMessageType('TrainRequest', (_message.Message,), dict(
  DESCRIPTOR = _TRAINREQUEST,
  __module__ = 'patchwork_pb2'
  # @@protoc_insertion_point(class_scope:TrainRequest)
  ))
_sym_db.RegisterMessage(TrainRequest)

TrainReply = _reflection.GeneratedProtocolMessageType('TrainReply', (_message.Message,), dict(
  DESCRIPTOR = _TRAINREPLY,
  __module__ = 'patchwork_pb2'
  # @@protoc_insertion_point(class_scope:TrainReply)
  ))
_sym_db.RegisterMessage(TrainReply)



_PATCHWORKSERVER = _descriptor.ServiceDescriptor(
  name='PatchworkServer',
  full_name='PatchworkServer',
  file=DESCRIPTOR,
  index=0,
  options=None,
  serialized_start=409,
  serialized_end=576,
  methods=[
  _descriptor.MethodDescriptor(
    name='GetStaticConfig',
    full_name='PatchworkServer.GetStaticConfig',
    index=0,
    containing_service=None,
    input_type=_STATICCONFIGREQUEST,
    output_type=_STATICCONFIGREPLY,
    options=None,
  ),
  _descriptor.MethodDescriptor(
    name='Evaluate',
    full_name='PatchworkServer.Evaluate',
    index=1,
    containing_service=None,
    input_type=_EVALUATEREQUEST,
    output_type=_EVALUATEREPLY,
    options=None,
  ),
  _descriptor.MethodDescriptor(
    name='Train',
    full_name='PatchworkServer.Train',
    index=2,
    containing_service=None,
    input_type=_TRAINREQUEST,
    output_type=_TRAINREPLY,
    options=None,
  ),
])
_sym_db.RegisterServiceDescriptor(_PATCHWORKSERVER)

DESCRIPTOR.services_by_name['PatchworkServer'] = _PATCHWORKSERVER

# @@protoc_insertion_point(module_scope)
