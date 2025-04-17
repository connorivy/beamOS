FROM python:3.13-slim as mkl_builder

RUN apt-get update && \
    apt-get install -y --no-install-recommends wget && \
    rm -rf /var/lib/apt/lists/*

RUN pip install mkl

FROM mcr.microsoft.com/devcontainers/dotnet:1-9.0-bookworm

# Create directory for the libraries in the destination image
ENV SOURCE_LIB_DIR=/usr/local/lib
ENV MKLROOT=/opt/mkl_libs
RUN mkdir -p ${MKLROOT}/lib/intel64

COPY --from=mkl_builder ${SOURCE_LIB_DIR}/lib* ${MKLROOT}/lib/intel64

# Add MKL libraries to library path
ENV LD_LIBRARY_PATH=${MKLROOT}/lib/intel64:$LD_LIBRARY_PATH
ENV LIBRARY_PATH=${MKLROOT}

# Verify the files were copied
RUN ls -la ${MKLROOT} && \
    ldconfig

WORKDIR /opt/mkl_libs/lib/intel64
RUN ln -s libmkl_gf_lp64.so.2 libmkl_gf_lp64.so 
RUN ln -s libmkl_gnu_thread.so.2 libmkl_gnu_thread.so
RUN ln -s libmkl_core.so.2 libmkl_core.so

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