﻿/*
 * Copyright (c) 2022 Steven Kuhn and contributors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
class PropertiesConverter : WriteOnlyJsonConverter<IDictionary<string, JsonElement>>
{
    public override void Write(VerifyJsonWriter writer, IDictionary<string, JsonElement> value)
    {
        writer.WriteStartObject();
        foreach (var item in value.OrderBy(x => x.Key))
        {
            writer.WritePropertyName(item.Key);

            if (item.Value.ValueKind == JsonValueKind.Null)
            {
                writer.WriteValue("null");
            }
            else
            {
                writer.WriteValue(item.Value.ToString());
            }
        }
        writer.WriteEndObject();
    }
}
