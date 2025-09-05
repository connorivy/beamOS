FROM python:3.13-slim AS mkl_builder

RUN apt-get update && \
    apt-get install -y --no-install-recommends wget && \
    rm -rf /var/lib/apt/lists/*

RUN pip install mkl

FROM mcr.microsoft.com/devcontainers/dotnet:1-10.0-preview-noble

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

ENV ConnectionStrings__BeamOsDb="Host=db;Port=5432;Database=postgres;Username=postgres;Password=postgres"
ENV BeamOsDevContainer="true"
ENV BEAMOS_ROOT="/workspaces/beamOS"

# # Install k6
# RUN gpg -k
# RUN gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
# RUN "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
# RUN apt-get update
# RUN apt-get install k6

RUN apt-get update && apt-get install -y gcc g++ gfortran

# # Install OpenSees directly from source. This takes a long time, but results in a faster OpenSees binary.
# # OpenSees dependencies
# RUN apt-get update && apt-get install -y gcc g++ gfortran

# RUN apt install -y liblapack-dev
# RUN apt install -y libopenmpi-dev
# RUN apt install -y libmkl-rt
# RUN apt install -y libmkl-blacs-openmpi-lp64
# RUN apt install -y libscalapack-openmpi-dev  
# RUN apt install -y libopenblas-dev  
# RUN apt install -y libeigen3-dev
# RUN apt install -y libblas-dev

# RUN apt install -y cmake
# RUN apt install -y python3-pip
# RUN apt install -y python3.12-venv
# RUN apt install -y git

# ENV HOME=/root
# WORKDIR $HOME

# # Build MUMPS
# WORKDIR /root
# RUN git clone https://github.com/OpenSees/mumps.git
# RUN cd mumps && mkdir build && cd build
# WORKDIR /root/mumps/build
# RUN cmake .. -Darith=d \
#     -DLAPACK_LIBRARY="/usr/lib/x86_64-linux-gnu/liblapack.so;/usr/lib/x86_64-linux-gnu/libblas.so" \
#     -DSCALAPACK_LIBRARY="/usr/lib/x86_64-linux-gnu/libscalapack-openmpi.so;/usr/lib/x86_64-linux-gnu/libblas.so" \
#     -DSCALAPACK_INCLUDE_DIR="/usr/include" \
#     -DBLAS_LIBRARIES="/usr/lib/x86_64-linux-gnu/libblas.so"
# RUN cmake --build . --config Release --parallel 4

# # Set up Python virtual environment and install Conan
# WORKDIR /root
# RUN python3 -m venv conan && \
#     . ./conan/bin/activate && \
#     python3 -m pip install "conan<2.0"

# # Build OpenSees
# WORKDIR /root
# RUN git clone https://github.com/connorivy/OpenSees.git && \
#     cd OpenSees && \
#     git checkout conan-changes && \
#     mkdir build && \
#     . ../conan/bin/activate && \
#     cd build && \
#     conan install .. --build missing 

# WORKDIR /root/OpenSees/build
# RUN cmake .. -DMUMPS_DIR=/root/mumps/build
# RUN cmake --build . --config Release --target OpenSees --parallel 4

# # Set the working directory to OpenSees/build (where the binaries are)
# WORKDIR /root/OpenSees/build

# # Add OpenSees binaries to PATH
# ENV PATH="/root/OpenSees/build:${PATH}"