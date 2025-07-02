using System.Collections.Generic;

namespace ServiceNow
{
    public abstract class ServiceNowBase
    {
        public enum SnField
        {
            sys_id,
            number,
            agent,
            topic,
            name,
            source,
            payload,
            assignment_group,
            count,
            user_name,
            phone,
            email,
            department,
            active,
            table_sys_id,
            table_name,
            file_name,
            content_type,
            u_outlook_name
        }
        public string TableName { get; set; }
        public string InstanceUrl { get; set; }
        public string[] ResultNames { get; set; }
        public string SysId { get; set; }

        public Dictionary<string, string> SnFields { get; set; } = new Dictionary<string, string>();

        public static void SetSnFieldValue(string field, string value, Dictionary<string, string> snFields)
        {
            if (string.IsNullOrEmpty(field)) return;

            if (string.IsNullOrEmpty(value))
            {
                snFields.Remove(field);
                return;
            }

            if (snFields.ContainsKey(field))
                snFields[field] = value;
            else
                snFields.Add(field, value);
        }
    }
}
