..\packages\Grpc.Tools.1.14.1\tools\windows_x86\protoc.exe --csharp_out . --grpc_out . patchwork.proto --plugin=protoc-gen-grpc=..\packages\Grpc.Tools.1.14.1\tools\windows_x86\grpc_csharp_plugin.exe

python -m grpc_tools.protoc --python_out=../python/PatchworkSim.Tensorforce/ --grpc_python_out=../python/PatchworkSim.Tensorforce/ -I. patchwork.proto