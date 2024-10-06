# Fluent bit

To setup fluent-bit, you need to setup the following items in Kubernetes:

1.  A daemonset             -- Daemonset definition for fluent bit
2.  A configmap             -- Defines 3 configuration files to be read by fluent bit
3.  A ServiceAccount        -- Defines a service account to be use by fluent bit to read Kubernetes metadata
4.  A ClusterRole           -- Defines a role to be use by service account to read Kubernetes metadata
5.  A ClusterRoleBinding    -- Binding cluster role to service account


## Volume mounts

Fluent bit cannot read logs represented by symlinks!
So we have to resolve the default kubernetes `/var/log/containers` container logs path, like so:

```sh
ls -l /var/log/containers
lrwxrwxrwx 1 root root  87 Oct  4 23:30 
    manjob1-jmqz2_default_test-job-b03cbd19bf699cd3f1c66a27e2edeb8f605e7cfd815e2ea7ae2194e53ecb30a0.log 
    -> /var/log/pods/default_manjob1-jmqz2_8e00090a-a3d7-4cbb-8cae-7e8afa9710ba/test-job/1.log

lrwxrwxrwx 1 root root 165 Oct  4 23:30 
    /var/log/pods/default_manjob1-jmqz2_8e00090a-a3d7-4cbb-8cae-7e8afa9710ba/test-job/1.log 
    -> /var/lib/docker/containers/b03cbd19bf699cd3f1c66a27e2edeb8f605e7cfd815e2ea7ae2194e53ecb30a0/b03cbd19bf699cd3f1c66a27e2edeb8f605e7cfd815e2ea7ae2194e53ecb30a0-json.log
```

So we know the logs are really stored at `/var/lib/docker/containers/`


## Service Account

Fluent bit not show Kubernetes-related metadata?

```log ; Your fluent bit logs probably will show something like:
[2024/10/06 00:49:23] [debug] [filter:kubernetes:kubernetes.0] Send out request to API Server for pods information
[2024/10/06 00:49:23] [debug] [http_client] not using http_proxy for header
[2024/10/06 00:49:23] [debug] [http_client] server kubernetes.default.svc:443 will close connection #94
[2024/10/06 00:49:23] [debug] [filter:kubernetes:kubernetes.0] Request (ns=default, pod=manjob3-t4d9l) http_do=0, HTTP Status: 403
[2024/10/06 00:49:23] [debug] [filter:kubernetes:kubernetes.0] HTTP response
{"kind":"Status","apiVersion":"v1","metadata":{},"status":"Failure","message":"pods \"manjob3-t4d9l\" is forbidden: User \"system:serviceaccount:default:default\" cannot get resource \"pods\" in API group \"\" in the namespace \"default\"","reason":"Forbidden","details":{"name":"manjob3-t4d9l","kind":"pods"},"code":403}
```

Reference: https://fluentbit.io/blog/2023/11/30/kubernetes-metadata-enrichment-with-fluent-bit-with-troubleshooting-tips/

# Configuration YAML

```yaml ; Filename: fluent-bit.configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  creationTimestamp: null
  name: fluent-bit
data:
  fluent-bit.conf: |
    [SERVICE]
        # Flush
        # =====
        # set an interval of seconds before to flush records to a destination
        flush        1

        # Daemon
        # ======
        # instruct Fluent Bit to run in foreground or background mode.
        daemon       Off

        # Log_Level
        # =========
        # Set the verbosity level of the service, values can be:
        #
        # - error
        # - warning
        # - info
        # - debug
        # - trace
        #
        # by default 'info' is set, that means it includes 'error' and 'warning'.
        log_level    debug

        # Parsers File
        # ============
        # specify an optional 'Parsers' configuration file
        parsers_file parsers.conf

        # Plugins File
        # ============
        # specify an optional 'Plugins' configuration file to load external plugins.
        plugins_file plugins.conf

        # HTTP Server
        # ===========
        # Enable/Disable the built-in HTTP Server for metrics
        http_server  Off
        http_listen  0.0.0.0
        http_port    2020

        # Storage
        # =======
        # Fluent Bit can use memory and filesystem buffering based mechanisms
        #
        # - https://docs.fluentbit.io/manual/administration/buffering-and-storage
        #
        # storage metrics
        # ---------------
        # publish storage pipeline metrics in '/api/v1/storage'. The metrics are
        # exported only if the 'http_server' option is enabled.
        #
        storage.metrics on

        # storage.path
        # ------------
        # absolute file system path to store filesystem data buffers (chunks).
        #
        # storage.path /tmp/storage

        # storage.sync
        # ------------
        # configure the synchronization mode used to store the data into the
        # filesystem. It can take the values normal or full.
        #
        # storage.sync normal

        # storage.checksum
        # ----------------
        # enable the data integrity check when writing and reading data from the
        # filesystem. The storage layer uses the CRC32 algorithm.
        #
        # storage.checksum off

        # storage.backlog.mem_limit
        # -------------------------
        # if storage.path is set, Fluent Bit will look for data chunks that were
        # not delivered and are still in the storage layer, these are called
        # backlog data. This option configure a hint of maximum value of memory
        # to use when processing these records.
        #
        # storage.backlog.mem_limit 5M

    [INPUT]
        Name              tail
        Tag               kube.*
        Path              /var/log/containers/manjob*.log
        Parser            docker
        DB                /var/log/flb_kube.db
        Mem_Buf_Limit     5MB
        Skip_Long_Lines   On
        Refresh_Interval  10

    # [INPUT]
    #     Name          systemd
    #     Path          /run/log/journal
    #     Parser        docker
    #     Tag           journal.*
    #     Mem_Buf_Limit 5MB        

    [FILTER]
        Name                kubernetes
        Match               kube.*
        # Merge_Log           On
        # Keep_Log            Off
        # K8S-Logging.Parser  On
        # K8S-Logging.Exclude On

    [OUTPUT]
        Name  stdout
        Match *

    [OUTPUT]
        Name            splunk
        Match           journal.*
        Host            sara
        Port            8088
        Splunk_Token    378b2325-088e-42f2-ab7b-7238368cc875
        TLS             on
        TLS.Verify      off

    [OUTPUT]
        Name            splunk
        Match           kube.*
        Host            sara
        Port            8088
        Splunk_Token    378b2325-088e-42f2-ab7b-7238368cc875
        tls             on
        tls.verify      off

  parsers.conf: |
    [PARSER]
        Name   apache
        Format regex
        Regex  ^(?<host>[^ ]*) [^ ]* (?<user>[^ ]*) \[(?<time>[^\]]*)\] "(?<method>\S+)(?: +(?<path>[^\"]*?)(?: +\S*)?)?" (?<code>[^ ]*) (?<size>[^ ]*)(?: "(?<referer>[^\"]*)" "(?<agent>[^\"]*)")?$
        Time_Key time
        Time_Format %d/%b/%Y:%H:%M:%S %z

    [PARSER]
        Name   apache2
        Format regex
        Regex  ^(?<host>[^ ]*) [^ ]* (?<user>[^ ]*) \[(?<time>[^\]]*)\] "(?<method>\S+)(?: +(?<path>[^ ]*) +\S*)?" (?<code>[^ ]*) (?<size>[^ ]*)(?: "(?<referer>[^\"]*)" "(?<agent>.*)")?$
        Time_Key time
        Time_Format %d/%b/%Y:%H:%M:%S %z

    [PARSER]
        Name   apache_error
        Format regex
        Regex  ^\[[^ ]* (?<time>[^\]]*)\] \[(?<level>[^\]]*)\](?: \[pid (?<pid>[^\]]*)\])?( \[client (?<client>[^\]]*)\])? (?<message>.*)$

    [PARSER]
        Name   nginx
        Format regex
        Regex ^(?<remote>[^ ]*) (?<host>[^ ]*) (?<user>[^ ]*) \[(?<time>[^\]]*)\] "(?<method>\S+)(?: +(?<path>[^\"]*?)(?: +\S*)?)?" (?<code>[^ ]*) (?<size>[^ ]*)(?: "(?<referer>[^\"]*)" "(?<agent>[^\"]*)")
        Time_Key time
        Time_Format %d/%b/%Y:%H:%M:%S %z

    [PARSER]
        # https://rubular.com/r/IhIbCAIs7ImOkc
        Name        k8s-nginx-ingress
        Format      regex
        Regex       ^(?<host>[^ ]*) - (?<user>[^ ]*) \[(?<time>[^\]]*)\] "(?<method>\S+)(?: +(?<path>[^\"]*?)(?: +\S*)?)?" (?<code>[^ ]*) (?<size>[^ ]*) "(?<referer>[^\"]*)" "(?<agent>[^\"]*)" (?<request_length>[^ ]*) (?<request_time>[^ ]*) \[(?<proxy_upstream_name>[^ ]*)\] (\[(?<proxy_alternative_upstream_name>[^ ]*)\] )?(?<upstream_addr>[^ ]*) (?<upstream_response_length>[^ ]*) (?<upstream_response_time>[^ ]*) (?<upstream_status>[^ ]*) (?<reg_id>[^ ]*).*$
        Time_Key    time
        Time_Format %d/%b/%Y:%H:%M:%S %z

    [PARSER]
        Name   json
        Format json
        Time_Key time
        Time_Format %d/%b/%Y:%H:%M:%S %z

    [PARSER]
        Name   logfmt
        Format logfmt

    [PARSER]
        Name         docker
        Format       json
        Time_Key     time
        Time_Format  %Y-%m-%dT%H:%M:%S.%L
        Time_Keep    On
        # --
        # Since Fluent Bit v1.2, if you are parsing Docker logs and using
        # the Kubernetes filter, it's not longer required to decode the
        # 'log' key.
        #
        # Command      |  Decoder | Field | Optional Action
        # =============|==================|=================
        #Decode_Field_As    json     log

    [PARSER]
        Name        docker-daemon
        Format      regex
        Regex       time="(?<time>[^ ]*)" level=(?<level>[^ ]*) msg="(?<msg>[^ ].*)"
        Time_Key    time
        Time_Format %Y-%m-%dT%H:%M:%S.%L
        Time_Keep   On

    [PARSER]
        Name        syslog-rfc5424
        Format      regex
        Regex       ^\<(?<pri>[0-9]{1,5})\>1 (?<time>[^ ]+) (?<host>[^ ]+) (?<ident>[^ ]+) (?<pid>[-0-9]+) (?<msgid>[^ ]+) (?<extradata>(\[(.*?)\]|-)) (?<message>.+)$
        Time_Key    time
        Time_Format %Y-%m-%dT%H:%M:%S.%L%z
        Time_Keep   On

    [PARSER]
        Name        syslog-rfc3164-local
        Format      regex
        Regex       ^\<(?<pri>[0-9]+)\>(?<time>[^ ]* {1,2}[^ ]* [^ ]*) (?<ident>[a-zA-Z0-9_\/\.\-]*)(?:\[(?<pid>[0-9]+)\])?(?:[^\:]*\:)? *(?<message>.*)$
        Time_Key    time
        Time_Format %b %d %H:%M:%S
        Time_Keep   On

    [PARSER]
        Name        syslog-rfc3164
        Format      regex
        Regex       /^\<(?<pri>[0-9]+)\>(?<time>[^ ]* {1,2}[^ ]* [^ ]*) (?<host>[^ ]*) (?<ident>[a-zA-Z0-9_\/\.\-]*)(?:\[(?<pid>[0-9]+)\])?(?:[^\:]*\:)? *(?<message>.*)$/
        Time_Key    time
        Time_Format %b %d %H:%M:%S
        Time_Keep   On

    [PARSER]
        Name    mongodb
        Format  regex
        Regex   ^(?<time>[^ ]*)\s+(?<severity>\w)\s+(?<component>[^ ]+)\s+\[(?<context>[^\]]+)]\s+(?<message>.*?) *(?<ms>(\d+))?(:?ms)?$
        Time_Format %Y-%m-%dT%H:%M:%S.%L
        Time_Keep   On
        Time_Key time

    [PARSER]
        # https://rubular.com/r/0VZmcYcLWMGAp1
        Name    envoy
        Format  regex
        Regex ^\[(?<start_time>[^\]]*)\] "(?<method>\S+)(?: +(?<path>[^\"]*?)(?: +\S*)?)? (?<protocol>\S+)" (?<code>[^ ]*) (?<response_flags>[^ ]*) (?<bytes_received>[^ ]*) (?<bytes_sent>[^ ]*) (?<duration>[^ ]*) (?<x_envoy_upstream_service_time>[^ ]*) "(?<x_forwarded_for>[^ ]*)" "(?<user_agent>[^\"]*)" "(?<request_id>[^\"]*)" "(?<authority>[^ ]*)" "(?<upstream_host>[^ ]*)"
        Time_Format %Y-%m-%dT%H:%M:%S.%L%z
        Time_Keep   On
        Time_Key start_time

    [PARSER]
        # https://rubular.com/r/17KGEdDClwiuDG
        Name    istio-envoy-proxy
        Format  regex
        Regex ^\[(?<start_time>[^\]]*)\] "(?<method>\S+)(?: +(?<path>[^\"]*?)(?: +\S*)?)? (?<protocol>\S+)" (?<response_code>[^ ]*) (?<response_flags>[^ ]*) (?<response_code_details>[^ ]*) (?<connection_termination_details>[^ ]*) (?<upstream_transport_failure_reason>[^ ]*) (?<bytes_received>[^ ]*) (?<bytes_sent>[^ ]*) (?<duration>[^ ]*) (?<x_envoy_upstream_service_time>[^ ]*) "(?<x_forwarded_for>[^ ]*)" "(?<user_agent>[^\"]*)" "(?<x_request_id>[^\"]*)" (?<authority>[^ ]*)" "(?<upstream_host>[^ ]*)" (?<upstream_cluster>[^ ]*) (?<upstream_local_address>[^ ]*) (?<downstream_local_address>[^ ]*) (?<downstream_remote_address>[^ ]*) (?<requested_server_name>[^ ]*) (?<route_name>[^  ]*)
        Time_Format %Y-%m-%dT%H:%M:%S.%L%z
        Time_Keep   On
        Time_Key start_time

    [PARSER]
        # http://rubular.com/r/tjUt3Awgg4
        Name cri
        Format regex
        Regex ^(?<time>[^ ]+) (?<stream>stdout|stderr) (?<logtag>[^ ]*) (?<message>.*)$
        Time_Key    time
        Time_Format %Y-%m-%dT%H:%M:%S.%L%z
        Time_Keep   On

    [PARSER]
        Name    kube-custom
        Format  regex
        Regex   (?<tag>[^.]+)?\.?(?<pod_name>[a-z0-9](?:[-a-z0-9]*[a-z0-9])?(?:\.[a-z0-9]([-a-z0-9]*[a-z0-9])?)*)_(?<namespace_name>[^_]+)_(?<container_name>.+)-(?<docker_id>[a-z0-9]{64})\.log$

    [PARSER]
        # Examples: TCP: https://rubular.com/r/Q8YY6fHqlqwGI0  UDP: https://rubular.com/r/B0ID69H9FvN0tp
        Name    kmsg-netfilter-log
        Format  regex
        Regex   ^\<(?<pri>[0-9]{1,5})\>1 (?<time>[^ ]+) (?<host>[^ ]+) kernel - - - \[[0-9\.]*\] (?<logprefix>[^ ]*)\s?IN=(?<in>[^ ]*) OUT=(?<out>[^ ]*) MAC=(?<macsrc>[0-9a-f]{2}:[0-9a-f]{2}:[0-9a-f]{2}:[0-9a-f]{2}:[0-9a-f]{2}:[0-9a-f]{2}):(?<macdst>[0-9a-f]{2}:[0-9a-f]{2}:[0-9a-f]{2}:[0-9a-f]{2}:[0-9a-f]{2}:[0-9a-f]{2}):(?<ethtype>[0-9a-f]{2}:[0-9a-f]{2}) SRC=(?<saddr>[^ ]*) DST=(?<daddr>[^ ]*) LEN=(?<len>[^ ]*) TOS=(?<tos>[^ ]*) PREC=(?<prec>[^ ]*) TTL=(?<ttl>[^ ]*) ID=(?<id>[^ ]*) (D*F*)\s*PROTO=(?<proto>[^ ]*)\s?((SPT=)?(?<sport>[0-9]*))\s?((DPT=)?(?<dport>[0-9]*))\s?((LEN=)?(?<protolen>[0-9]*))\s?((WINDOW=)?(?<window>[0-9]*))\s?((RES=)?(?<res>0?x?[0-9]*))\s?(?<flag>[^ ]*)\s?((URGP=)?(?<urgp>[0-9]*))
        Time_Key  time
        Time_Format  %Y-%m-%dT%H:%M:%S.%L%z

  plugins.conf: |
    [PLUGINS]
        # Path /path/to/out_gstdout.so

  appsettings.json: |
    {
        "Logging": {
            "LogLevel": {
                "Default": "Information"
            }
        },
        "Serilog": {
            "Using": [ "Serilog.Sinks.File", "Serilog.Enrichers.Environment" ],
            "MinimumLevel": {
                "Default": "Information"
            },
            "WriteTo": [
                {
                    "Name": "Console",
                    "Args": {
                        "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Sixteen, Serilog.Sinks.Console",
                        "applyThemeToRedirectedOutput": true
                    }
                }
            ],
            "Enrich": [ "FromLogContext", "WithMachineName" ],
            "Properties": {
                "ApplicationName": "SimpleJobConsoleApp"
            }
        },
        "FeatureFlags": {
            "USE_DUMMY_FLAG": false,
            "TEST_EXCEPTION_FLAG": false
        }
    }
```

```yaml ; fluent-bit.daemonset.yaml
apiVersion: apps/v1
kind: DaemonSet
metadata:
  creationTimestamp: null
  labels:
    app: fluent-bit
  name: fluent-bit
spec:
  selector:
    matchLabels:
      app: fluent-bit
  template:
    metadata:
      creationTimestamp: null
      labels:
        app: fluent-bit
    spec:
      # securityContext:
      #   runAsUser: 0
      serviceAccountName: fluent-bit
      volumes:
        - name: fluent-bit-config
          configMap:
            name: fluent-bit
        - name: varlog
          hostPath:
            path: /var/log
        - name: varlibdockercontainers
          hostPath:
            path: /var/lib/docker/containers
        # - name: mnt
        #   hostPath:
        #     path: /mnt
        - name: journal
          hostPath:
            path: /run/log/journal
      containers:
      - image: docker.io/fluent/fluent-bit:3.1.9
        name: fluent-bit
        ports:
          - containerPort: 2020
        env:
          - name: FLUENT_ELASTICSEARCH_HOST
            value: "elasticsearch"
          - name: FLUENT_ELASTICSEARCH_PORT
            value: "9200"
        volumeMounts:
          - name: fluent-bit-config
            mountPath: /fluent-bit/etc/
          - name: varlog
            mountPath: /var/log
          - name: varlibdockercontainers
            mountPath: /var/lib/docker/containers
            readOnly: true
          # - name: mnt
          #   mountPath: /mnt
          #   readOnly: true
          - name: journal
            mountPath: /run/log/journal
        resources: {}


```

```yaml ; fluent-bit.rbac.yaml
apiVersion: v1
kind: ServiceAccount
metadata:
  name: fluent-bit
---
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRole
metadata:
  name: fluent-bit-role
rules:
- apiGroups: [""]
  resources: ["pods"]
  verbs: ["get", "list", "watch"]
---
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRoleBinding
metadata:
  name: fluent-bit-role-binding
subjects:
- kind: ServiceAccount
  name: fluent-bit
  namespace: default
roleRef:
  kind: ClusterRole
  name: fluent-bit-role
  apiGroup: rbac.authorization.k8s.io
```

# Reference

https://docs.fluentbit.io/manual/pipeline/filters/kubernetes
https://github.com/fluent/fluent-bit-kubernetes-logging/blob/master/output/elasticsearch/fluent-bit-configmap.yaml