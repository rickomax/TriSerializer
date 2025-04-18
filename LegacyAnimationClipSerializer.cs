using System;
using System.Collections;
using UnityEngine;

namespace TriSerializer
{
    public class LegacyAnimationClipSerializer : ObjectSerializer<AnimationClip>
    {
        public override ObjectIdentifier Identifier => "LAC";

        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }

            var isLegacy = serializationContext.BinaryReader.ReadBoolean();
            if (!isLegacy)
            {
                yield break;
            }

            throw new NotImplementedException();
        }

        public override IEnumerable Serialize(SerializationContext serializationContext, AnimationClip source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }

            if (!source.legacy)
            {
                serializationContext.BinaryWriter.Write(false);
                yield break;
            }
            else
            {
                serializationContext.BinaryWriter.Write(true);
            }

            serializationContext.BinaryWriter.Write(source.name);

            var events = source.events;
            if (events != null)
            {
                serializationContext.BinaryWriter.Write(events.Length);
                foreach (var @event in events)
                {
                    serializationContext.BinaryWriter.Write(@event.time);
                    serializationContext.BinaryWriter.Write(@event.functionName);
                    serializationContext.BinaryWriter.Write(@event.stringParameter);
                    serializationContext.BinaryWriter.Write(@event.floatParameter);
                    serializationContext.BinaryWriter.Write(@event.intParameter);
                    serializationContext.WriteReference(@event.objectReferenceParameter);
                    serializationContext.BinaryWriter.Write((int)@event.messageOptions);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(0);
            }

            serializationContext.BinaryWriter.Write(source.frameRate);
            serializationContext.BinaryWriter.Write(source.localBounds);
        }
    }
}