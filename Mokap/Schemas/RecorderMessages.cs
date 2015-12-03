// automatically generated, do not modify

namespace Mokap.Schemas.RecorderMessages
{

using FlatBuffers;

public enum MessageIds : int
{
 Metadata = 1,
 BodyFrameData = 2,
 ColorFrameData = 3,
 DepthFrameData = 4,
};

public sealed class Metadata : Table {
  public static Metadata GetRootAsMetadata(ByteBuffer _bb) { return GetRootAsMetadata(_bb, new Metadata()); }
  public static Metadata GetRootAsMetadata(ByteBuffer _bb, Metadata obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Metadata __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int ColorFrameWidth { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ColorFrameHeight { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int DepthFrameWidth { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int DepthFrameHeight { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<Metadata> CreateMetadata(FlatBufferBuilder builder,
      int colorFrameWidth = 0,
      int colorFrameHeight = 0,
      int depthFrameWidth = 0,
      int depthFrameHeight = 0) {
    builder.StartObject(4);
    Metadata.AddDepthFrameHeight(builder, depthFrameHeight);
    Metadata.AddDepthFrameWidth(builder, depthFrameWidth);
    Metadata.AddColorFrameHeight(builder, colorFrameHeight);
    Metadata.AddColorFrameWidth(builder, colorFrameWidth);
    return Metadata.EndMetadata(builder);
  }

  public static void StartMetadata(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddColorFrameWidth(FlatBufferBuilder builder, int colorFrameWidth) { builder.AddInt(0, colorFrameWidth, 0); }
  public static void AddColorFrameHeight(FlatBufferBuilder builder, int colorFrameHeight) { builder.AddInt(1, colorFrameHeight, 0); }
  public static void AddDepthFrameWidth(FlatBufferBuilder builder, int depthFrameWidth) { builder.AddInt(2, depthFrameWidth, 0); }
  public static void AddDepthFrameHeight(FlatBufferBuilder builder, int depthFrameHeight) { builder.AddInt(3, depthFrameHeight, 0); }
  public static Offset<Metadata> EndMetadata(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Metadata>(o);
  }
};

public sealed class BodyFrameData : Table {
  public static BodyFrameData GetRootAsBodyFrameData(ByteBuffer _bb) { return GetRootAsBodyFrameData(_bb, new BodyFrameData()); }
  public static BodyFrameData GetRootAsBodyFrameData(ByteBuffer _bb, BodyFrameData obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public BodyFrameData __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public long RelativeTime { get { int o = __offset(4); return o != 0 ? bb.GetLong(o + bb_pos) : (long)0; } }
  public Body GetBodies(int j) { return GetBodies(new Body(), j); }
  public Body GetBodies(Body obj, int j) { int o = __offset(6); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BodiesLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<BodyFrameData> CreateBodyFrameData(FlatBufferBuilder builder,
      long relativeTime = 0,
      VectorOffset bodiesOffset = default(VectorOffset)) {
    builder.StartObject(2);
    BodyFrameData.AddRelativeTime(builder, relativeTime);
    BodyFrameData.AddBodies(builder, bodiesOffset);
    return BodyFrameData.EndBodyFrameData(builder);
  }

  public static void StartBodyFrameData(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddRelativeTime(FlatBufferBuilder builder, long relativeTime) { builder.AddLong(0, relativeTime, 0); }
  public static void AddBodies(FlatBufferBuilder builder, VectorOffset bodiesOffset) { builder.AddOffset(1, bodiesOffset.Value, 0); }
  public static VectorOffset CreateBodiesVector(FlatBufferBuilder builder, Offset<Body>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBodiesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<BodyFrameData> EndBodyFrameData(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<BodyFrameData>(o);
  }
};

public sealed class Body : Table {
  public static Body GetRootAsBody(ByteBuffer _bb) { return GetRootAsBody(_bb, new Body()); }
  public static Body GetRootAsBody(ByteBuffer _bb, Body obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Body __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ulong TrackingId { get { int o = __offset(4); return o != 0 ? bb.GetUlong(o + bb_pos) : (ulong)0; } }
  public bool IsTracked { get { int o = __offset(6); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool IsRestricted { get { int o = __offset(8); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int ClippedEdges { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public Hand HandLeft { get { return GetHandLeft(new Hand()); } }
  public Hand GetHandLeft(Hand obj) { int o = __offset(12); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public Hand HandRight { get { return GetHandRight(new Hand()); } }
  public Hand GetHandRight(Hand obj) { int o = __offset(14); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public Joint GetJoints(int j) { return GetJoints(new Joint(), j); }
  public Joint GetJoints(Joint obj, int j) { int o = __offset(16); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int JointsLength { get { int o = __offset(16); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Body> CreateBody(FlatBufferBuilder builder,
      ulong trackingId = 0,
      bool isTracked = false,
      bool isRestricted = false,
      int clippedEdges = 0,
      Offset<Hand> handLeftOffset = default(Offset<Hand>),
      Offset<Hand> handRightOffset = default(Offset<Hand>),
      VectorOffset jointsOffset = default(VectorOffset)) {
    builder.StartObject(7);
    Body.AddTrackingId(builder, trackingId);
    Body.AddJoints(builder, jointsOffset);
    Body.AddHandRight(builder, handRightOffset);
    Body.AddHandLeft(builder, handLeftOffset);
    Body.AddClippedEdges(builder, clippedEdges);
    Body.AddIsRestricted(builder, isRestricted);
    Body.AddIsTracked(builder, isTracked);
    return Body.EndBody(builder);
  }

  public static void StartBody(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddTrackingId(FlatBufferBuilder builder, ulong trackingId) { builder.AddUlong(0, trackingId, 0); }
  public static void AddIsTracked(FlatBufferBuilder builder, bool isTracked) { builder.AddBool(1, isTracked, false); }
  public static void AddIsRestricted(FlatBufferBuilder builder, bool isRestricted) { builder.AddBool(2, isRestricted, false); }
  public static void AddClippedEdges(FlatBufferBuilder builder, int clippedEdges) { builder.AddInt(3, clippedEdges, 0); }
  public static void AddHandLeft(FlatBufferBuilder builder, Offset<Hand> handLeftOffset) { builder.AddOffset(4, handLeftOffset.Value, 0); }
  public static void AddHandRight(FlatBufferBuilder builder, Offset<Hand> handRightOffset) { builder.AddOffset(5, handRightOffset.Value, 0); }
  public static void AddJoints(FlatBufferBuilder builder, VectorOffset jointsOffset) { builder.AddOffset(6, jointsOffset.Value, 0); }
  public static VectorOffset CreateJointsVector(FlatBufferBuilder builder, Offset<Joint>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartJointsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Body> EndBody(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Body>(o);
  }
};

public sealed class Hand : Table {
  public static Hand GetRootAsHand(ByteBuffer _bb) { return GetRootAsHand(_bb, new Hand()); }
  public static Hand GetRootAsHand(ByteBuffer _bb, Hand obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Hand __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Confidence { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int State { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<Hand> CreateHand(FlatBufferBuilder builder,
      int confidence = 0,
      int state = 0) {
    builder.StartObject(2);
    Hand.AddState(builder, state);
    Hand.AddConfidence(builder, confidence);
    return Hand.EndHand(builder);
  }

  public static void StartHand(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddConfidence(FlatBufferBuilder builder, int confidence) { builder.AddInt(0, confidence, 0); }
  public static void AddState(FlatBufferBuilder builder, int state) { builder.AddInt(1, state, 0); }
  public static Offset<Hand> EndHand(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Hand>(o);
  }
};

public sealed class Joint : Table {
  public static Joint GetRootAsJoint(ByteBuffer _bb) { return GetRootAsJoint(_bb, new Joint()); }
  public static Joint GetRootAsJoint(ByteBuffer _bb, Joint obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Joint __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Type { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int State { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public Vector2 Position2D { get { return GetPosition2D(new Vector2()); } }
  public Vector2 GetPosition2D(Vector2 obj) { int o = __offset(8); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public Vector3 Position3D { get { return GetPosition3D(new Vector3()); } }
  public Vector3 GetPosition3D(Vector3 obj) { int o = __offset(10); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public Vector4 Rotation { get { return GetRotation(new Vector4()); } }
  public Vector4 GetRotation(Vector4 obj) { int o = __offset(12); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }

  public static Offset<Joint> CreateJoint(FlatBufferBuilder builder,
      int type = 0,
      int state = 0,
      Offset<Vector2> position2DOffset = default(Offset<Vector2>),
      Offset<Vector3> position3DOffset = default(Offset<Vector3>),
      Offset<Vector4> rotationOffset = default(Offset<Vector4>)) {
    builder.StartObject(5);
    Joint.AddRotation(builder, rotationOffset);
    Joint.AddPosition3D(builder, position3DOffset);
    Joint.AddPosition2D(builder, position2DOffset);
    Joint.AddState(builder, state);
    Joint.AddType(builder, type);
    return Joint.EndJoint(builder);
  }

  public static void StartJoint(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddType(FlatBufferBuilder builder, int type) { builder.AddInt(0, type, 0); }
  public static void AddState(FlatBufferBuilder builder, int state) { builder.AddInt(1, state, 0); }
  public static void AddPosition2D(FlatBufferBuilder builder, Offset<Vector2> position2DOffset) { builder.AddOffset(2, position2DOffset.Value, 0); }
  public static void AddPosition3D(FlatBufferBuilder builder, Offset<Vector3> position3DOffset) { builder.AddOffset(3, position3DOffset.Value, 0); }
  public static void AddRotation(FlatBufferBuilder builder, Offset<Vector4> rotationOffset) { builder.AddOffset(4, rotationOffset.Value, 0); }
  public static Offset<Joint> EndJoint(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Joint>(o);
  }
};

public sealed class Vector2 : Table {
  public static Vector2 GetRootAsVector2(ByteBuffer _bb) { return GetRootAsVector2(_bb, new Vector2()); }
  public static Vector2 GetRootAsVector2(ByteBuffer _bb, Vector2 obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Vector2 __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public double X { get { int o = __offset(4); return o != 0 ? bb.GetDouble(o + bb_pos) : (double)0; } }
  public double Y { get { int o = __offset(6); return o != 0 ? bb.GetDouble(o + bb_pos) : (double)0; } }

  public static Offset<Vector2> CreateVector2(FlatBufferBuilder builder,
      double x = 0,
      double y = 0) {
    builder.StartObject(2);
    Vector2.AddY(builder, y);
    Vector2.AddX(builder, x);
    return Vector2.EndVector2(builder);
  }

  public static void StartVector2(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddX(FlatBufferBuilder builder, double x) { builder.AddDouble(0, x, 0); }
  public static void AddY(FlatBufferBuilder builder, double y) { builder.AddDouble(1, y, 0); }
  public static Offset<Vector2> EndVector2(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Vector2>(o);
  }
};

public sealed class Vector3 : Table {
  public static Vector3 GetRootAsVector3(ByteBuffer _bb) { return GetRootAsVector3(_bb, new Vector3()); }
  public static Vector3 GetRootAsVector3(ByteBuffer _bb, Vector3 obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Vector3 __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public double X { get { int o = __offset(4); return o != 0 ? bb.GetDouble(o + bb_pos) : (double)0; } }
  public double Y { get { int o = __offset(6); return o != 0 ? bb.GetDouble(o + bb_pos) : (double)0; } }
  public double Z { get { int o = __offset(8); return o != 0 ? bb.GetDouble(o + bb_pos) : (double)0; } }

  public static Offset<Vector3> CreateVector3(FlatBufferBuilder builder,
      double x = 0,
      double y = 0,
      double z = 0) {
    builder.StartObject(3);
    Vector3.AddZ(builder, z);
    Vector3.AddY(builder, y);
    Vector3.AddX(builder, x);
    return Vector3.EndVector3(builder);
  }

  public static void StartVector3(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddX(FlatBufferBuilder builder, double x) { builder.AddDouble(0, x, 0); }
  public static void AddY(FlatBufferBuilder builder, double y) { builder.AddDouble(1, y, 0); }
  public static void AddZ(FlatBufferBuilder builder, double z) { builder.AddDouble(2, z, 0); }
  public static Offset<Vector3> EndVector3(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Vector3>(o);
  }
};

public sealed class Vector4 : Table {
  public static Vector4 GetRootAsVector4(ByteBuffer _bb) { return GetRootAsVector4(_bb, new Vector4()); }
  public static Vector4 GetRootAsVector4(ByteBuffer _bb, Vector4 obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Vector4 __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public double X { get { int o = __offset(4); return o != 0 ? bb.GetDouble(o + bb_pos) : (double)0; } }
  public double Y { get { int o = __offset(6); return o != 0 ? bb.GetDouble(o + bb_pos) : (double)0; } }
  public double Z { get { int o = __offset(8); return o != 0 ? bb.GetDouble(o + bb_pos) : (double)0; } }
  public double W { get { int o = __offset(10); return o != 0 ? bb.GetDouble(o + bb_pos) : (double)0; } }

  public static Offset<Vector4> CreateVector4(FlatBufferBuilder builder,
      double x = 0,
      double y = 0,
      double z = 0,
      double w = 0) {
    builder.StartObject(4);
    Vector4.AddW(builder, w);
    Vector4.AddZ(builder, z);
    Vector4.AddY(builder, y);
    Vector4.AddX(builder, x);
    return Vector4.EndVector4(builder);
  }

  public static void StartVector4(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddX(FlatBufferBuilder builder, double x) { builder.AddDouble(0, x, 0); }
  public static void AddY(FlatBufferBuilder builder, double y) { builder.AddDouble(1, y, 0); }
  public static void AddZ(FlatBufferBuilder builder, double z) { builder.AddDouble(2, z, 0); }
  public static void AddW(FlatBufferBuilder builder, double w) { builder.AddDouble(3, w, 0); }
  public static Offset<Vector4> EndVector4(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Vector4>(o);
  }
};

public sealed class ColorFrameData : Table {
  public static ColorFrameData GetRootAsColorFrameData(ByteBuffer _bb) { return GetRootAsColorFrameData(_bb, new ColorFrameData()); }
  public static ColorFrameData GetRootAsColorFrameData(ByteBuffer _bb, ColorFrameData obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ColorFrameData __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public long RelativeTime { get { int o = __offset(4); return o != 0 ? bb.GetLong(o + bb_pos) : (long)0; } }
  public int Width { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Height { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public byte GetData(int j) { int o = __offset(10); return o != 0 ? bb.Get(__vector(o) + j * 1) : (byte)0; }
  public int DataLength { get { int o = __offset(10); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ColorFrameData> CreateColorFrameData(FlatBufferBuilder builder,
      long relativeTime = 0,
      int width = 0,
      int height = 0,
      VectorOffset dataOffset = default(VectorOffset)) {
    builder.StartObject(4);
    ColorFrameData.AddRelativeTime(builder, relativeTime);
    ColorFrameData.AddData(builder, dataOffset);
    ColorFrameData.AddHeight(builder, height);
    ColorFrameData.AddWidth(builder, width);
    return ColorFrameData.EndColorFrameData(builder);
  }

  public static void StartColorFrameData(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddRelativeTime(FlatBufferBuilder builder, long relativeTime) { builder.AddLong(0, relativeTime, 0); }
  public static void AddWidth(FlatBufferBuilder builder, int width) { builder.AddInt(1, width, 0); }
  public static void AddHeight(FlatBufferBuilder builder, int height) { builder.AddInt(2, height, 0); }
  public static void AddData(FlatBufferBuilder builder, VectorOffset dataOffset) { builder.AddOffset(3, dataOffset.Value, 0); }
  public static VectorOffset CreateDataVector(FlatBufferBuilder builder, byte[] data) { builder.StartVector(1, data.Length, 1); for (int i = data.Length - 1; i >= 0; i--) builder.AddByte(data[i]); return builder.EndVector(); }
  public static void StartDataVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(1, numElems, 1); }
  public static Offset<ColorFrameData> EndColorFrameData(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ColorFrameData>(o);
  }
};

public sealed class DepthFrameData : Table {
  public static DepthFrameData GetRootAsDepthFrameData(ByteBuffer _bb) { return GetRootAsDepthFrameData(_bb, new DepthFrameData()); }
  public static DepthFrameData GetRootAsDepthFrameData(ByteBuffer _bb, DepthFrameData obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DepthFrameData __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public long RelativeTime { get { int o = __offset(4); return o != 0 ? bb.GetLong(o + bb_pos) : (long)0; } }
  public int Width { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Height { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public ushort GetData(int j) { int o = __offset(10); return o != 0 ? bb.GetUshort(__vector(o) + j * 2) : (ushort)0; }
  public int DataLength { get { int o = __offset(10); return o != 0 ? __vector_len(o) : 0; } }
  public ushort MinReliableDistance { get { int o = __offset(12); return o != 0 ? bb.GetUshort(o + bb_pos) : (ushort)0; } }
  public ushort MaxReliableDistance { get { int o = __offset(14); return o != 0 ? bb.GetUshort(o + bb_pos) : (ushort)0; } }

  public static Offset<DepthFrameData> CreateDepthFrameData(FlatBufferBuilder builder,
      long relativeTime = 0,
      int width = 0,
      int height = 0,
      VectorOffset dataOffset = default(VectorOffset),
      ushort minReliableDistance = 0,
      ushort maxReliableDistance = 0) {
    builder.StartObject(6);
    DepthFrameData.AddRelativeTime(builder, relativeTime);
    DepthFrameData.AddData(builder, dataOffset);
    DepthFrameData.AddHeight(builder, height);
    DepthFrameData.AddWidth(builder, width);
    DepthFrameData.AddMaxReliableDistance(builder, maxReliableDistance);
    DepthFrameData.AddMinReliableDistance(builder, minReliableDistance);
    return DepthFrameData.EndDepthFrameData(builder);
  }

  public static void StartDepthFrameData(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddRelativeTime(FlatBufferBuilder builder, long relativeTime) { builder.AddLong(0, relativeTime, 0); }
  public static void AddWidth(FlatBufferBuilder builder, int width) { builder.AddInt(1, width, 0); }
  public static void AddHeight(FlatBufferBuilder builder, int height) { builder.AddInt(2, height, 0); }
  public static void AddData(FlatBufferBuilder builder, VectorOffset dataOffset) { builder.AddOffset(3, dataOffset.Value, 0); }
  public static VectorOffset CreateDataVector(FlatBufferBuilder builder, ushort[] data) { builder.StartVector(2, data.Length, 2); for (int i = data.Length - 1; i >= 0; i--) builder.AddUshort(data[i]); return builder.EndVector(); }
  public static void StartDataVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(2, numElems, 2); }
  public static void AddMinReliableDistance(FlatBufferBuilder builder, ushort minReliableDistance) { builder.AddUshort(4, minReliableDistance, 0); }
  public static void AddMaxReliableDistance(FlatBufferBuilder builder, ushort maxReliableDistance) { builder.AddUshort(5, maxReliableDistance, 0); }
  public static Offset<DepthFrameData> EndDepthFrameData(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DepthFrameData>(o);
  }
};


}
