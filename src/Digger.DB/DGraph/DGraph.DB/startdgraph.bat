@echo off

start ./dgraph.exe zero
start ./dgraph.exe server --lru_mb 2048
start ./dgraph-ratel.exe