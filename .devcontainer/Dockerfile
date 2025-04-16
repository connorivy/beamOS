FROM python:3.13-slim as mkl_builder

RUN apt-get update && \
    apt-get install -y --no-install-recommends wget && \
    rm -rf /var/lib/apt/lists/*

RUN pip install mkl

FROM mcr.microsoft.com/devcontainers/dotnet:1-9.0-bookworm

# Create directory for the libraries in the destination image
ENV SOURCE_LIB_DIR=/usr/local/lib
ENV DEST_LIB_DIR=/opt/mkl_libs
RUN mkdir -p ${DEST_LIB_DIR}

COPY --from=mkl_builder ${SOURCE_LIB_DIR}/lib* ${DEST_LIB_DIR}/

# Add MKL libraries to library path
ENV LD_LIBRARY_PATH=${DEST_LIB_DIR}:$LD_LIBRARY_PATH

# Verify the files were copied
RUN ls -la ${DEST_LIB_DIR} && \
    ldconfig

# OpenSees dependencies
RUN apt-get update && apt-get install -y gcc g++ gfortran

# sudo apt install -y cmake
# sudo apt install -y gcc g++ gfortran
# sudo apt install -y python3-pip
# sudo apt install -y liblapack-dev
# sudo apt install -y libopenmpi-dev
# sudo apt install -y libmkl-rt
# sudo apt install -y libmkl-blacs-openmpi-lp64
# sudo apt install -y libscalapack-openmpi-dev

# Install CUDA for GPU acceleration (optional but recommended for Ollama)
# RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
#     && apt-get install -y --no-install-recommends \
#     software-properties-common \
#     && add-apt-repository contrib \
#     && apt-get update \
#     && apt-get install -y --no-install-recommends \
#     nvidia-cuda-toolkit

# [Optional] Uncomment to install additional OS packages
# RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
#     && apt-get -y install --no-install-recommends <your-package-list-here>