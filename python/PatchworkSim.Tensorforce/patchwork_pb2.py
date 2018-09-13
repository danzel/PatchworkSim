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
  serialized_pb=_b('\n\x0fpatchwork.proto\"\x15\n\x13StaticConfigRequest\",\n\x11StaticConfigReply\x12\x17\n\x0fobservationSize\x18\x01 \x01(\x05\")\n\rCreateRequest\x12\x18\n\x10opponentStrength\x18\x01 \x01(\x05\"@\n\x0b\x43reateReply\x12\x0e\n\x06gameId\x18\x01 \x01(\x05\x12!\n\x0bobservation\x18\x02 \x01(\x0b\x32\x0c.Observation\"+\n\x0bMoveRequest\x12\x0e\n\x06gameId\x18\x01 \x01(\x05\x12\x0c\n\x04move\x18\x02 \x01(\x05\"[\n\tMoveReply\x12\x14\n\x0cgameHasEnded\x18\x01 \x01(\x08\x12\x15\n\rwinningPlayer\x18\x02 \x01(\x05\x12!\n\x0bobservation\x18\x03 \x01(\x0b\x32\x0c.Observation\"=\n\x0bObservation\x12\x0e\n\x06reward\x18\x01 \x01(\x02\x12\x1e\n\x16observationForNextMove\x18\x02 \x03(\x02\x32\xa5\x01\n\x0fPatchworkServer\x12=\n\x0fGetStaticConfig\x12\x14.StaticConfigRequest\x1a\x12.StaticConfigReply\"\x00\x12(\n\x06\x43reate\x12\x0e.CreateRequest\x1a\x0c.CreateReply\"\x00\x12)\n\x0bPerformMove\x12\x0c.MoveRequest\x1a\n.MoveReply\"\x00\x62\x06proto3')
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


_CREATEREQUEST = _descriptor.Descriptor(
  name='CreateRequest',
  full_name='CreateRequest',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='opponentStrength', full_name='CreateRequest.opponentStrength', index=0,
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
  serialized_start=88,
  serialized_end=129,
)


_CREATEREPLY = _descriptor.Descriptor(
  name='CreateReply',
  full_name='CreateReply',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='gameId', full_name='CreateReply.gameId', index=0,
      number=1, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='observation', full_name='CreateReply.observation', index=1,
      number=2, type=11, cpp_type=10, label=1,
      has_default_value=False, default_value=None,
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
  serialized_start=131,
  serialized_end=195,
)


_MOVEREQUEST = _descriptor.Descriptor(
  name='MoveRequest',
  full_name='MoveRequest',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='gameId', full_name='MoveRequest.gameId', index=0,
      number=1, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='move', full_name='MoveRequest.move', index=1,
      number=2, type=5, cpp_type=1, label=1,
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
  serialized_start=197,
  serialized_end=240,
)


_MOVEREPLY = _descriptor.Descriptor(
  name='MoveReply',
  full_name='MoveReply',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='gameHasEnded', full_name='MoveReply.gameHasEnded', index=0,
      number=1, type=8, cpp_type=7, label=1,
      has_default_value=False, default_value=False,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='winningPlayer', full_name='MoveReply.winningPlayer', index=1,
      number=2, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='observation', full_name='MoveReply.observation', index=2,
      number=3, type=11, cpp_type=10, label=1,
      has_default_value=False, default_value=None,
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
  serialized_start=242,
  serialized_end=333,
)


_OBSERVATION = _descriptor.Descriptor(
  name='Observation',
  full_name='Observation',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='reward', full_name='Observation.reward', index=0,
      number=1, type=2, cpp_type=6, label=1,
      has_default_value=False, default_value=float(0),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='observationForNextMove', full_name='Observation.observationForNextMove', index=1,
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
  serialized_start=335,
  serialized_end=396,
)

_CREATEREPLY.fields_by_name['observation'].message_type = _OBSERVATION
_MOVEREPLY.fields_by_name['observation'].message_type = _OBSERVATION
DESCRIPTOR.message_types_by_name['StaticConfigRequest'] = _STATICCONFIGREQUEST
DESCRIPTOR.message_types_by_name['StaticConfigReply'] = _STATICCONFIGREPLY
DESCRIPTOR.message_types_by_name['CreateRequest'] = _CREATEREQUEST
DESCRIPTOR.message_types_by_name['CreateReply'] = _CREATEREPLY
DESCRIPTOR.message_types_by_name['MoveRequest'] = _MOVEREQUEST
DESCRIPTOR.message_types_by_name['MoveReply'] = _MOVEREPLY
DESCRIPTOR.message_types_by_name['Observation'] = _OBSERVATION
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

CreateRequest = _reflection.GeneratedProtocolMessageType('CreateRequest', (_message.Message,), dict(
  DESCRIPTOR = _CREATEREQUEST,
  __module__ = 'patchwork_pb2'
  # @@protoc_insertion_point(class_scope:CreateRequest)
  ))
_sym_db.RegisterMessage(CreateRequest)

CreateReply = _reflection.GeneratedProtocolMessageType('CreateReply', (_message.Message,), dict(
  DESCRIPTOR = _CREATEREPLY,
  __module__ = 'patchwork_pb2'
  # @@protoc_insertion_point(class_scope:CreateReply)
  ))
_sym_db.RegisterMessage(CreateReply)

MoveRequest = _reflection.GeneratedProtocolMessageType('MoveRequest', (_message.Message,), dict(
  DESCRIPTOR = _MOVEREQUEST,
  __module__ = 'patchwork_pb2'
  # @@protoc_insertion_point(class_scope:MoveRequest)
  ))
_sym_db.RegisterMessage(MoveRequest)

MoveReply = _reflection.GeneratedProtocolMessageType('MoveReply', (_message.Message,), dict(
  DESCRIPTOR = _MOVEREPLY,
  __module__ = 'patchwork_pb2'
  # @@protoc_insertion_point(class_scope:MoveReply)
  ))
_sym_db.RegisterMessage(MoveReply)

Observation = _reflection.GeneratedProtocolMessageType('Observation', (_message.Message,), dict(
  DESCRIPTOR = _OBSERVATION,
  __module__ = 'patchwork_pb2'
  # @@protoc_insertion_point(class_scope:Observation)
  ))
_sym_db.RegisterMessage(Observation)



_PATCHWORKSERVER = _descriptor.ServiceDescriptor(
  name='PatchworkServer',
  full_name='PatchworkServer',
  file=DESCRIPTOR,
  index=0,
  options=None,
  serialized_start=399,
  serialized_end=564,
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
    name='Create',
    full_name='PatchworkServer.Create',
    index=1,
    containing_service=None,
    input_type=_CREATEREQUEST,
    output_type=_CREATEREPLY,
    options=None,
  ),
  _descriptor.MethodDescriptor(
    name='PerformMove',
    full_name='PatchworkServer.PerformMove',
    index=2,
    containing_service=None,
    input_type=_MOVEREQUEST,
    output_type=_MOVEREPLY,
    options=None,
  ),
])
_sym_db.RegisterServiceDescriptor(_PATCHWORKSERVER)

DESCRIPTOR.services_by_name['PatchworkServer'] = _PATCHWORKSERVER

# @@protoc_insertion_point(module_scope)