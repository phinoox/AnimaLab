#! /bin/sh

FILE=$1

if [ ! -f "$FILE" ]; then
    echo "file $FILE not found" >&2
    exit 1
fi

echo "*" $FILE \n >> ./tmp/overview.md
grep -rn 'public class\|public enum' $FILE | sed 's/^/\t \* /' >> ./tmp/overview.md