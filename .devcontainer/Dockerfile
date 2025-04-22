FROM python:3.13-slim AS mkl_builder

RUN apt-get update && \
    apt-get install -y --no-install-recommends wget && \
    rm -rf /var/lib/apt/lists/*

RUN pip install mkl

FROM mcr.microsoft.com/devcontainers/dotnet:1-9.0-noble

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
RUN apt install -y cmake
RUN apt install -y python3-pip
RUN apt install -y python3.12-venv
RUN apt install -y liblapack-dev
RUN apt install -y libopenmpi-dev
RUN apt install -y libmkl-rt
RUN apt install -y libmkl-blacs-openmpi-lp64
RUN apt install -y libscalapack-openmpi-dev  
RUN apt install -y git

RUN apt install -y libopenblas-dev  
RUN apt install -y libeigen3-dev
RUN apt install -y libblas-dev

ENV HOME=/root
WORKDIR $HOME

# Build MUMPS
WORKDIR /root
RUN git clone https://github.com/OpenSees/mumps.git
RUN cd mumps && mkdir build && cd build
WORKDIR /root/mumps/build
RUN cmake .. -Darith=d \
    -DLAPACK_LIBRARY="/usr/lib/x86_64-linux-gnu/liblapack.so;/usr/lib/x86_64-linux-gnu/libblas.so" \
    -DSCALAPACK_LIBRARY="/usr/lib/x86_64-linux-gnu/libscalapack-openmpi.so;/usr/lib/x86_64-linux-gnu/libblas.so" \
    -DSCALAPACK_INCLUDE_DIR="/usr/include" \
    -DBLAS_LIBRARIES="/usr/lib/x86_64-linux-gnu/libblas.so"
RUN cmake --build . --config Release --parallel 4

# Build HDF5
WORKDIR /root
RUN git clone --depth 1 --branch hdf5-1_12_2 https://github.com/HDFGroup/hdf5.git && \
    cd hdf5 && \
    ./configure --prefix=/usr/local/hdf5 && \
    make -j 4 && \
    make install && \
    ldconfig

# # Set up Python virtual environment and install Conan
# WORKDIR /root
# RUN python3 -m venv conan && \
#     . ./conan/bin/activate && \
#     python3 -m pip install conan && \
#     conan profile detect --force

# Set up Python virtual environment and install Conan
WORKDIR /root
RUN python3 -m venv conan && \
    . ./conan/bin/activate && \
    python3 -m pip install "conan<2.0"

ENV LD_LIBRARY_PATH=/usr/local/hdf5/lib:$LD_LIBRARY_PATH
ENV LIBRARY_PATH=/usr/local/hdf5/lib:$LIBRARY_PATH
ENV CONAN_LIB_DIRS=/usr/local/hdf5/lib:$CONAN_LIB_DIRS
# Build OpenSees
WORKDIR /root
RUN git clone https://github.com/connorivy/OpenSees.git && \
    cd OpenSees && \
    git checkout conan-changes && \
    mkdir build && \
    . ../conan/bin/activate && \
    cd build && \
    # conan install conan.txt --output-folder=build --build=missing
    conan install .. --build missing 

WORKDIR /root/OpenSees/build
RUN cmake .. -DMUMPS_DIR=/root/mumps/build
RUN cmake --build . --config Release --target OpenSees --parallel 4
# RUN cmake .. \
#     -DCMAKE_TOOLCHAIN_FILE=conan_toolchain.cmake \
#     -DCMAKE_BUILD_TYPE=Release \
#     -DHDF5_C_LIBRARY=/usr/local/hdf5/lib/libhdf5.so \
#     -DHDF5_CPP_LIBRARY=/usr/local/hdf5/lib/libhdf5_cpp.so \
#     -DHDF5_HL_CPP_LIBRARY=/usr/local/hdf5/lib/libhdf5_hl_cpp.so \
#     -DMUMPS_DIR=/root/mumps/build \
#     -DOPENMPI=TRUE \
#     -DSCALAPACK_LIBRARIES="/usr/lib/x86_64-linux-gnu/libmkl_blacs_openmpi_lp64.so;/usr/lib/x86_64-linux-gnu/libscalapack-openmpi.so" \
#     -DPython_LIBRARIES=/usr/lib/x86_64-linux-gnu/libpython3.10.so \
#     -DPython_INCLUDE_DIRS=/usr/include/python3.10 \
#     -DLAPACK_LIBRARIES="-m64 -Wl,--start-group /usr/lib/x86_64-linux-gnu/libmkl_intel_ilp64.so /usr/lib/x86_64-linux-gnu/libmkl_sequential.so /usr/lib/x86_64-linux-gnu/libmkl_core.so -Wl,--end-group -lpthread" && \
#     cmake --build . --config Release --target OpenSees --parallel 4


# cmake --build . --config Release --target OpenSeesPy --parallel 4 && \
# cmake --build . --config Release --target OpenSeesSP --parallel 4 && \
# cmake --build . --config Release --target OpenSeesMP --parallel 4

# Set the working directory to OpenSees/build (where the binaries are)
WORKDIR /root/OpenSees/build

# Add OpenSees binaries to PATH
ENV PATH="/root/OpenSees/build:${PATH}"

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